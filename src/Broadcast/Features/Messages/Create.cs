using System;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Dtos.Messages;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Services.Messages;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Messages
{
    public class Create
    {
        public class Command : IRequest<MessageEnvelope>
        {
            public MessageDto Message { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Message).NotNull().SetValidator(new MessageValidator());
            }
        }

        public class Handler : IRequestHandler<Command, MessageEnvelope>
        {
            private readonly IMessageService _messageService;

            public Handler(IMessageService messageService)
            {
                _messageService = messageService;
            }

            public async Task<MessageEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var message = await _messageService.InsertMessageAsync(request.Message);
                    return new MessageEnvelope(message.ToDto<MessageDto>());
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}