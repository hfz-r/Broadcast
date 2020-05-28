using System;
using Newtonsoft.Json;

namespace Broadcast.Core.Dtos.Users
{
    public class UserDto
    {
        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("designation")]
        public string Designation { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }
    }
}