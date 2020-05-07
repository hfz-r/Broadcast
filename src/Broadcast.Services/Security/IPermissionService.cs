using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Security;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure.Security;

namespace Broadcast.Services.Security
{
    public interface IPermissionService
    {
        Task<bool> AuthorizeAsync(Permission permission);
        Task<bool> AuthorizeAsync(Permission permission, User user);
        Task<bool> AuthorizeAsync(string permissionName);
        Task<bool> AuthorizeAsync(string permissionName, User user);
        Task DeletePermissionAsync(Permission permission);
        Task<Permission> GetPermissionByIdAsync(int permissionId);
        Task<Permission> GetPermissionByNameAsync(string name);
        Task<IList<Permission>> GetPermissionsAsync();
        Task InsertPermissionAsync(Permission permission);
        Task InstallPermissionsAsync(IPermissionProvider permissionProvider);
        Task UpdatePermissionAsync(Permission permission);

        Task InstallPermissionsAsync1(IList<JsonToObject> obj);
    }
}