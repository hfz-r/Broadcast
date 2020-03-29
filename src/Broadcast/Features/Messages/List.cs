using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain;
using Broadcast.Core.Domain.Messages;
using Broadcast.Dtos.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Services.Common;
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
            private readonly IGenericAttributeService _genericAttributeService;
            private readonly ICurrentUserAccessor _currentUser;

            public QueryHandler(IUnitOfWork worker, IGenericAttributeService genericAttributeService,
                ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _genericAttributeService = genericAttributeService;
                _currentUser = currentUser;
            }

            public async Task<MessagesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Message>();

                try
                {
                    var messages = await repo.GetPagedListAsync(
                        orderBy: src => src.OrderByDescending(m => m.CreatedAt), index: message.Offset ?? 0,
                        size: message.Limit ?? 20,
                        cancellationToken: cancellationToken);

                    if (messages == null) throw new ArgumentNullException(nameof(messages));

                    var items = messages.Items.Select(msg =>
                    {
                        var dto = msg.ToDto<MessageDto>();
                        dto.ProjectDto.Ngc = GetAttribute<bool>(msg, string.Format(MessageDefaults.LocationAttribute, "Ngc"));
                        dto.ProjectDto.Hsb = GetAttribute<bool>(msg, string.Format(MessageDefaults.LocationAttribute, "Hsb"));
                        dto.ProjectDto.Pj = GetAttribute<bool>(msg, string.Format(MessageDefaults.LocationAttribute, "Pj"));

                        return dto;
                    });

                    return new MessagesEnvelope
                    {
                        Messages = items.ToList(),
                        MessagesCount = messages.Count
                    };
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                T GetAttribute<T>(BaseEntity entity, string key)
                {
                    return AsyncHelper.RunSync(() => _genericAttributeService.GetAttributeAsync<T>(entity, key));
                }
            }
        }
    }
}