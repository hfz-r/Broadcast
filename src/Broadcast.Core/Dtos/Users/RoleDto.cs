using Newtonsoft.Json;

namespace Broadcast.Core.Dtos.Users
{
    public class RoleDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}