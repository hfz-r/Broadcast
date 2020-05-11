using Broadcast.Core;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Tags;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Messages;
using Broadcast.Dtos.Projects;
using Broadcast.Infrastructure;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Domain = Broadcast.Core.Domain.Projects;

namespace Broadcast.Features.Project
{
    public class Create
    {
        public class Command : IRequest<Project.ProjectEnvelope>
        {
            public ProjectDto Project { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Project).NotNull().SetValidator(new ProjectValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Project.ProjectEnvelope>
        {
            private readonly IUnitOfWork _worker;
            private readonly ICurrentUserAccessor _currentUser;

            public Handler(IUnitOfWork worker, ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _currentUser = currentUser;
            }

            public async Task<Project.ProjectEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var userRepo = _worker.GetRepositoryAsync<User>();
                var projRepo = _worker.GetRepositoryAsync<Domain.Project>();

                if ((await projRepo.GetQueryableAsync(m => m.Name == request.Project.Project)).Any())
                    throw new RestException(HttpStatusCode.BadRequest,
                   new { Project = $"Project {Constants.Existed}", Title = $"Title {Constants.Existed}" });

                var project = new Domain.Project
                {
                    Name = request.Project.Project,
                    Description = request.Project.Description,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = _currentUser.CurrentUser.Name,
                    Modifiedby = String.Empty,
                    ModifiedOn = DateTime.MinValue,
                };
                await projRepo.AddAsync(project);
                await _worker.SaveChangesAsync();
                return new Project.ProjectEnvelope(project.ToDto<ProjectDto>());
            }
        }
    }
}
