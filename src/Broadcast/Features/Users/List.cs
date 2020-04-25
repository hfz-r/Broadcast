using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Core.Infrastructure.Paging;
using Broadcast.Dtos.Users;
using MediatR;

namespace Broadcast.Features.Users
{
    public class List
    {
        public class Query : IRequest<UsersEnvelope>
        {
            public Query(string department, int? limit, int? offset)
            {
                Department = department;
                Limit = limit;
                Offset = offset;
            }

            public string Department { get; }

            public int? Limit { get; }

            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, UsersEnvelope>
        {
            private readonly IUnitOfWork _worker;

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<UsersEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<User>();

                var users = await repo.GetQueryableAsync(queryExp: src =>
                {
                    if (!string.IsNullOrEmpty(request.Department)) src = src.Where(user => user.Department.Contains(request.Department));
                    return src;
                }, orderBy: src => src.OrderByDescending(u => u.Id));

                if (users == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Users = $"Users {Constants.NotFound}"});

                var paginateUser = await users.ToPaginateAsync(request.Offset ?? 0, request.Limit ?? 20, 0, cancellationToken);
                return new UsersEnvelope
                {
                    Users = paginateUser.Items.Select(user => user.ToDto<UserDto>()).ToList(),
                    UsersCount = paginateUser.Count
                };
            }
        }
    }
}