using System;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Services.Caching.CachingDefaults;
using Broadcast.Services.Caching.Extensions;

namespace Broadcast.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _worker;

        public UserService(IUnitOfWork worker)
        {
            _worker = worker;
        }

        #region Roles

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var repo = _worker.GetRepositoryAsync<Role>();
            var key = string.Format(UserServiceCachingDefaults.RolesByNameCacheKey, name);

            var query = await repo.GetQueryableAsync(role => role.Name == name, role => role.OrderBy(r => r.Id));
            return await query.ToCachedFirstOrDefaultAsync(key);
        }

        public async Task InsertRoleAsync(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var repo = _worker.GetRepositoryAsync<Role>();
            await repo.AddAsync(role);

            await _worker.SaveChangesAsync();
        }

        #endregion
    }
}