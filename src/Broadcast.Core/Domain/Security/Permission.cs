using System.Collections.Generic;

namespace Broadcast.Core.Domain.Security
{
    public class Permission : BaseEntity
    {
        private ICollection<PermissionRole> _permissionRoles;

        public string Name { get; set; }

        public string Category { get; set; }

        public virtual ICollection<PermissionRole> PermissionRoles
        {
            get => _permissionRoles ?? (_permissionRoles = new List<PermissionRole>());
            protected set => _permissionRoles = value;
        }
    }
}