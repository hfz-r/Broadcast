using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class ProjectDto
    {
        [JsonProperty("project")]
        public string Project { get; set; }
    }
}