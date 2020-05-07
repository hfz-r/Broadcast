using Newtonsoft.Json;

namespace Broadcast.Dtos.Users
{
    public class RoleDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}