using Newtonsoft.Json;

namespace Broadcast.Core.Dtos.Messages
{
    public class DetailsDto
    {
        [JsonProperty("editor")]
        public string Editor { get; set; }
    }
}