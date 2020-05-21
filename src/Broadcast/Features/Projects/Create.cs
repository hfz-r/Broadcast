using Broadcast.Core;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Dtos.Projects;
using Broadcast.Core.Domain.Projects;

namespace Broadcast.Features.Projects
{
    public class Create
    {
        public class Command : IRequest<ProjectEnvelope>
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

        public class Handler : IRequestHandler<Command, ProjectEnvelope>
        {
            private readonly IUnitOfWork _worker;
            private readonly ICurrentUserAccessor _currentUser;

            public Handler(IUnitOfWork worker, ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _currentUser = currentUser;
            }

            public async Task<ProjectEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var projRepo = _worker.GetRepositoryAsync<Project>();

                if ((await projRepo.GetQueryableAsync(m => m.Name == request.Project.Project)).Any())
                    throw new RestException(HttpStatusCode.BadRequest, new { Project = $"Project {Constants.Existed}", Title = $"Title {Constants.Existed}" });

                var project = new Project
                {
                    Name = request.Project.Project,
                    Description = request.Project.Description,
                    Slug = request.Project.Project?.GenerateSlug(),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = _currentUser.CurrentUser?.AccountName,
                    ModifiedBy = _currentUser.CurrentUser?.AccountName,
                    ModifiedOn = DateTime.UtcNow,
                };

                await projRepo.AddAsync(project, cancellationToken);

                await _worker.SaveChangesAsync();

                return new ProjectEnvelope(project.ToDto<ProjectDto>());
            }
        }
    }
}
