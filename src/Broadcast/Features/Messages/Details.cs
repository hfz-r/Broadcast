using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Messages;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Messages
{
    public class Details
    {
        public class Query : IRequest<MessageEnvelope>
        {
            public Query(string slug)
            {
                Slug = slug;
            }

            public string Slug { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, MessageEnvelope>
        {
            private readonly IUnitOfWork _worker;

            public QueryHandler(IUnitOfWork worker)
            {
                _worker = worker;
            }

            public async Task<MessageEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var repo = _worker.GetRepositoryAsync<Message>();

                var message = await repo.SingleAsync(predicate: msg => msg.Slug == request.Slug);
                if (message == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Message = $"Message {Constants.NotFound}"});

                return new MessageEnvelope(message.ToDto<MessageDto>());
            }
        }
    }
}