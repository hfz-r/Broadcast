using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Projects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Features.Project
{
    public class List
    {
        public class Query : IRequest<Project.ProjectsEnvelope>
        {
            public Query(string name, int size)
            {
                Name = name;
                Size = size;
            }

            public string Name { get; }
            public int? Size { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Project.ProjectsEnvelope>
        {
            private readonly IUnitOfWork _worker;

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<Project.ProjectsEnvelope> Handle(Query project, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Core.Domain.Projects.Project>();

                var projects = await repo.GetPagedListAsync(
                    orderBy: x => x.OrderByDescending(mbox => mbox.CreatedOn),
                    index: 0,
                    size:project.Size ?? 20,
                    cancellationToken: cancellationToken);

                if (projects == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Messages = $"Messages {Constants.NotFound}" });

                return new Project.ProjectsEnvelope
                {
                    Projects = projects.Items.Select(proj => proj.ToDto<ProjectDto>()).ToList(),
                    ProjectsCount = projects.Items.Count()
                };
            }
        }
    }
}
