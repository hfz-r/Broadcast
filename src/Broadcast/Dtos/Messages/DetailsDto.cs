using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class DetailsDto
    {
        [JsonProperty("editor")]
        public string Editor { get; set; }
    }
}