using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Dtos.Projects;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Projects
{
    public class Edit
    {
        public class Command : IRequest<ProjectEnvelope>
        {
            public string Slug { get; set; }

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
                var repo = _worker.GetRepositoryAsync<Project>();

                var project = await repo.SingleAsync(x => x.Slug == request.Slug);
                if (project == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Project = $"Project {Constants.NotFound}"});

                project.Name = request.Project.Project ?? project.Name;
                project.Description = request.Project.Description ?? project.Description;
                project.ModifiedOn = DateTime.UtcNow;
                project.ModifiedBy = _currentUser.CurrentUser?.AccountName;

                await _worker.SaveChangesAsync();

                return new ProjectEnvelope(project.ToDto<ProjectDto>());
            }
        }
    }
}