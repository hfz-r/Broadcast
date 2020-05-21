using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using MediatR;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Dtos.Projects;

namespace Broadcast.Features.Projects
{
    public class List
    {
        public class Query : IRequest<ProjectsEnvelope>
        {
            public Query(string name, int? size, int? offset)
            {
                Name = name;
                Size = size;
                Offset = offset;
            }

            public string Name { get; }

            public int? Size { get; set; }

            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, ProjectsEnvelope>
        {
            private readonly IUnitOfWork _worker;

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<ProjectsEnvelope> Handle(Query project, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Project>();

                var projects = await repo.GetPagedListAsync(
                    orderBy: x => x.OrderByDescending(mbox => mbox.CreatedOn),
                    index: project.Offset ?? 0,
                    size: project.Size ?? 20,
                    cancellationToken: cancellationToken);

                if (projects == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Messages = $"Messages {Constants.NotFound}"});

                return new ProjectsEnvelope
                {
                    Projects = projects.Items.Select(proj => proj.ToDto<ProjectDto>()).ToList(),
                    ProjectsCount = projects.Count
                };
            }
        }
    }
}