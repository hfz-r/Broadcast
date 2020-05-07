using Broadcast.Core.Domain.Users;

namespace Broadcast.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission-user role mapping class
    /// </summary>
    public class PermissionRole : BaseEntity
    {
        public int PermissionId { get; set; }

        public int RoleId { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual Role Role { get; set; }
    }
}