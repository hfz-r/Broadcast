using System.Collections.Generic;

namespace Broadcast.Features.Messages
{
    public class MessageEnvelope
    {
        public MessageEnvelope(Domain.Message message)
        {
            Message = message;
        }

        public Domain.Message Message { get; }
    }

    public class MessagesEnvelope
    {
        public IList<Domain.Message> Messages { get; set; }

        public int MessagesCount { get; set; }
    }
}