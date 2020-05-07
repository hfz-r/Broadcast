using System.Collections.Generic;
using Broadcast.Dtos.Users;
using Newtonsoft.Json;

namespace Broadcast.Dtos.Security
{
    public class SecurityDto
    {
        [JsonProperty("role")]
        public RoleDto Role { get; set; }

        [JsonProperty("users")]
        public IList<UserDto> Users { get; set; }

        [JsonProperty("permissions")]
        public IList<PermissionDto> Permissions { get; set; }
    }
}