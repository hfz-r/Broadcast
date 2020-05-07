using Broadcast.Core.Domain.Users;

namespace Broadcast.Core.Domain.Security
{
    public class PermissionRole1 : BaseEntity
    {
        public int RoleId { get; set; }

        public virtual Permission1 Permission { get; set; }

        public virtual Role Role { get; set; }
    }
}