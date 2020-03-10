using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Infrastructure;
using Broadcast.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Broadcast.Features.Messages
{
    public class List
    {
        public class Query : IRequest<MessagesEnvelope>
        {
            public Query(string tag, string author, int? limit, int? offset)
            {
                Tag = tag;
                Author = author;
                Limit = limit;
                Offset = offset;
            }

            public string Tag { get; }

            public string Author { get; }

            public int? Limit { get; }

            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, MessagesEnvelope>
        {
            private readonly IUnitOfWork _worker;
            private readonly ICurrentUserAccessor _currentUser;

            public QueryHandler(IUnitOfWork worker, ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _currentUser = currentUser;
            }

            public async Task<MessagesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Domain.Message>();

                var messages = await repo.GetPagedListAsync(
                    include: src => src.Include(m => m.Author).Include(m => m.MessageTags), 
                    orderBy: src => src.OrderByDescending(m => m.CreatedAt), index: message.Offset ?? 0,
                    size: message.Limit ?? 20,
                    cancellationToken: cancellationToken);

                return new MessagesEnvelope
                {
                    Messages = messages.Items.Randomize().ToList(),
                    MessagesCount = messages.Count  
                };
            }
        }
    }
}