using System.Collections.Generic;
using Broadcast.Core.Dtos.Messages;
using Newtonsoft.Json;

namespace Broadcast.Features.Messages
{
    public class MessageEnvelope
    {
        public MessageEnvelope(MessageDto message)
        {
            Message = message;
        }

        [JsonProperty("message")]
        public MessageDto Message { get; }
    }

    public class MessagesEnvelope
    {
        [JsonProperty("messages")]
        public List<MessageDto> Messages { get; set; }

        [JsonProperty("messageCount")]
        public int MessagesCount { get; set; }
    }
}