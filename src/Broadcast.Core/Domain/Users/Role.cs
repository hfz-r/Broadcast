using System.Collections.Generic;
using Broadcast.Core.Domain.Security;

namespace Broadcast.Core.Domain.Users
{
    public class Role : BaseEntity
    {
        private ICollection<PermissionRole1> _permissionRoles1;

        public string Name { get; set; }

        //todo - create roles-permission from json
        public virtual ICollection<PermissionRole1> PermissionRoles1
        {
            get => _permissionRoles1 ?? (_permissionRoles1 = new List<PermissionRole1>());
            protected set => _permissionRoles1 = value;
        }
    }
}