using System.Collections.Generic;
using Broadcast.Core.Dtos.Users;
using Newtonsoft.Json;

namespace Broadcast.Features.Users
{
    public class UserEnvelope
    {
        public UserEnvelope(UserDto user)
        {
            User = user;
        }

        [JsonProperty("user")]
        public UserDto User { get; set; }
    }

    public class UsersEnvelope
    {
        [JsonProperty("users")]
        public List<UserDto> Users { get; set; }

        [JsonProperty("userCount")]
        public int UsersCount { get; set; }
    }
}