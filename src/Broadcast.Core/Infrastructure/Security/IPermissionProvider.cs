using System.Collections.Generic;
using Broadcast.Core.Domain.Security;

namespace Broadcast.Core.Infrastructure.Security
{
    public interface IPermissionProvider
    {
        HashSet<(string roleName, Permission[] permissions)> GetDefaultPermissions();
        IEnumerable<Permission> GetPermissions();
    }
}