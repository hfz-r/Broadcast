using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Security;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Dtos.Security;
using Broadcast.Core.Dtos.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Core.Infrastructure.Paging;
using MediatR;

namespace Broadcast.Features.Security
{
    public class List
    {
        public class Query : IRequest<SecuritiesEnvelope>
        {
            public Query(string username, int? permissionId, int? limit, int? offset)
            {
                Username = username;
                PermissionId = permissionId;
                Limit = limit;
                Offset = offset;
            }

            public string Username { get; set; }

            public int? PermissionId { get; set; }

            public int? Limit { get; }

            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, SecuritiesEnvelope>
        {
            private readonly IUnitOfWork _worker;

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<SecuritiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var roleRepo = _worker.GetRepositoryAsync<Role>();
                var userRoleRepo = _worker.GetRepositoryAsync<UserRole>();
                var permissionRoleRepo = _worker.GetRepositoryAsync<PermissionRole>();

                var role = await roleRepo.GetQueryableAsync();
                var userRole = await userRoleRepo.GetQueryableAsync();
                var permissionRole = await permissionRoleRepo.GetQueryableAsync();

                //todo - expensive operation; should make selective queries and compute at frontend
                var securities = role
                    .GroupJoin(userRole, r => r.Id, ur => ur.RoleId, (r, ur) => new { r, ur })
                    .GroupJoin(permissionRole, r => r.r.Id, pr => pr.RoleId, (r, pr) => new { r.r, r.ur, pr })
                    .Select(r => new SecurityDto
                    {
                        Role = r.r.ToDto<RoleDto>(),
                        Users = r.ur.Select(x => x.User.ToDto<UserDto>()).ToList(),
                        Permissions = r.pr.Select(x => x.Permission.ToDto<PermissionDto>()).ToList()
                    });

                if (securities == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Securities = $"Securities {Constants.NotFound}" });

                var paginateSec = await securities.ToPaginateAsync(request.Offset ?? 0, request.Limit ?? 20, 0, cancellationToken);
                return new SecuritiesEnvelope
                {
                    Securities = paginateSec.Items.ToList(),
                    SecuritiesCount = paginateSec.Count
                };
            }
        }
    }
}