using Newtonsoft.Json;

namespace Broadcast.Dtos.Users
{
    public class UserDto
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
