using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Domain.Security;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Caching.CachingDefaults;
using Broadcast.Services.Caching.Extensions;
using Broadcast.Services.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Broadcast.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUnitOfWork _worker;
        private readonly IUserService _userService;
        private readonly ICurrentUserAccessor _userAccessor;

        public PermissionService(
            IStaticCacheManager staticCacheManager,
            IUnitOfWork worker,
            IUserService userService,
            ICurrentUserAccessor userAccessor)
        {
            _staticCacheManager = staticCacheManager;
            _worker = worker;
            _userService = userService;
            _userAccessor = userAccessor;
        }

        #region Utilities

        protected async Task<bool> AuthorizeAsync(string permissionName, int roleId)
        {
            if (string.IsNullOrEmpty(permissionName)) return false;

            var key = string.Format(SecurityCachingDefaults.PermissionsAllowedCacheKey, roleId, permissionName);
            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionByRoleId(roleId);
                return permissions.Any(permission =>
                    permission.Name.Equals(permissionName, StringComparison.InvariantCultureIgnoreCase));
            });
        }

        protected async Task<IList<Permission>> GetPermissionByRoleId(int roleId)
        {
            var key = string.Format(SecurityCachingDefaults.PermissionsByRoleIdCacheKey, roleId);

            var permissions = await (_worker.GetRepositoryAsync<Permission>()).GetQueryableAsync();
            var permissionRole = await (_worker.GetRepositoryAsync<PermissionRole>()).GetQueryableAsync();

            var query = from p in permissions
                join pr in permissionRole on p.Id equals pr.PermissionId
                where pr.RoleId == roleId
                orderby p.Id
                select p;

            return await query.ToCachedListAsync(key);
        }

        #endregion

        public async Task DeletePermissionAsync(Permission permission)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));

            var repo = _worker.GetRepositoryAsync<Permission>();
            await repo.DeleteAsync(permission);

            await _worker.SaveChangesAsync();
        }

        public async Task<Permission> GetPermissionByIdAsync(int permissionId)
        {
            if (permissionId == 0) return null;

            var repo = _worker.GetRepositoryAsync<Permission>();
            return await repo.ToCachedGetByIdAsync(permissionId);
        }

        public async Task<Permission> GetPermissionByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var repo = _worker.GetRepositoryAsync<Permission>();
            return await repo.SingleAsync(
                permission => permission.Name == name,
                permission => permission.OrderBy(p => p.Id));
        }

        public async Task<IList<Permission>> GetPermissionsAsync()
        {
            var repo = _worker.GetRepositoryAsync<Permission>();
            return (await repo.GetQueryableAsync()).ToList();
        }

        public async Task InsertPermissionAsync(Permission permission)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));

            var repo = _worker.GetRepositoryAsync<Permission>();
            await repo.AddAsync(permission);

            await _worker.SaveChangesAsync();
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));

            var repo = _worker.GetRepositoryAsync<Permission>();
            repo.Update(permission);

            await _worker.SaveChangesAsync();
        }

        //todo - create roles-permission from json
        public async Task InstallPermissionsAsync1(IList<JsonToObject> obj)
        {
            var repo = _worker.GetRepositoryAsync<PermissionRole1>();

            //var path = Path.GetFullPath(Path.Combine("../Broadcast.Services/Security", "permission-role.json"));
            //var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //var path = $"{directory}\\Domain\\Security\\permission-role.json";
            //var jsonText = File.ReadAllText(path, Encoding.UTF8);
            //if (string.IsNullOrEmpty(jsonText)) throw new ArgumentNullException(nameof(jsonText));

            //var obj = JsonConvert.DeserializeObject<IList<JsonToObject>>(jsonText, new StringEnumConverter());
            //if (obj == null) throw new ArgumentNullException(nameof(obj));

            foreach (var src in obj)
            {
                var role = await _userService.GetRoleByNameAsync(src.Role);
                if (role == null)
                {
                    //new role (save it)
                    role = new Role {Name = src.Role};
                    await _userService.InsertRoleAsync(role);
                }

                foreach (var vvv in src.Permissions)
                {
                    if ((await repo.GetQueryableAsync(role1 => role1.RoleId == role.Id && role1.Permission != vvv)).Any())
                    {
                        role.PermissionRoles1.Add(new PermissionRole1 { Permission = vvv });
                    }
                }
            }
        }

        public async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionByNameAsync(permission.Name);
                if (permission1 != null) continue;

                permission1 = new Permission
                {
                    Name = permission.Name,
                    Category = permission.Category
                };

                foreach (var defaultPermission in defaultPermissions)
                {
                    var role = await _userService.GetRoleByNameAsync(defaultPermission.roleName);
                    if (role == null)
                    {
                        //new role (save it)
                        role = new Role {Name = defaultPermission.roleName};
                        await _userService.InsertRoleAsync(role);
                    }

                    var defaultMappingProvided = defaultPermission.permissions.Any(dp => dp.Name == permission1.Name);
                    if (defaultMappingProvided)
                    {
                        permission1.PermissionRoles.Add(new PermissionRole {Role = role});
                    }
                }

                await InsertPermissionAsync(permission1);
            }
        }

        #region Authorize

        public async Task<bool> AuthorizeAsync(Permission permission)
        {
            return await AuthorizeAsync(permission, _userAccessor.CurrentUser);
        }

        public async Task<bool> AuthorizeAsync(Permission permission, User user)
        {
            if (permission == null) return false;
            if (user == null) return false;

            return await AuthorizeAsync(permission.Name, user);
        }

        public async Task<bool> AuthorizeAsync(string permissionName)
        {
            return await AuthorizeAsync(permissionName, _userAccessor.CurrentUser);
        }

        public async Task<bool> AuthorizeAsync(string permissionName, User user)
        {
            if (string.IsNullOrEmpty(permissionName)) return false;

            var roles = user.UserRoles.Select(ur => ur.Role);
            foreach (var role in roles)
                if (await AuthorizeAsync(permissionName, role.Id)) return true;

            return false;
        }

        #endregion
    }
}