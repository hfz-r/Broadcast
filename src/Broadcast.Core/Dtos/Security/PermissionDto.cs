using Newtonsoft.Json;

namespace Broadcast.Core.Dtos.Security
{
    public class PermissionDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}