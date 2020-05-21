using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Dtos.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using MediatR;

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

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<MessagesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Message>();

                var messages = await repo.GetPagedListAsync(
                    orderBy: src => src.OrderByDescending(m => m.CreatedAt),
                    index: message.Offset ?? 0,
                    size: message.Limit ?? 20,
                    cancellationToken: cancellationToken);

                if (messages == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Messages = $"Messages {Constants.NotFound}"});

                return new MessagesEnvelope
                {
                    Messages = messages.Items.Select(msg => msg.ToDto<MessageDto>()).ToList(),
                    MessagesCount = messages.Count
                };
            }
        }
    }
}