using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class ProjectDto
    {
        [JsonProperty("project")]
        public string Project { get; set; }

        public LocationDto LocationDto { get; set; }
    }

    public class LocationDto
    {
        [JsonProperty("NGC")]
        public bool Ngc { get; set; }

        [JsonProperty("HSB")]
        public bool Hsb { get; set; }

        [JsonProperty("PJ")]
        public bool Pj { get; set; }
    }
}