using System.Collections.Generic;
using Broadcast.Domain;
using Broadcast.Dtos.Messages;

namespace Broadcast.Features.Messages
{
    public class MessageEnvelope
    {
        public MessageEnvelope(MessageDto message)
        {
            Message = message;
        }

        public MessageDto Message { get; }
    }

    public class MessagesEnvelope
    {
        public List<MessageDto> Messages { get; set; }

        public int MessagesCount { get; set; }
    }
}