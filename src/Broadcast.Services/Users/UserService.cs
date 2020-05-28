using System;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Dtos.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Services.Caching.CachingDefaults;
using Broadcast.Services.Caching.Extensions;
using Broadcast.Services.Common;

namespace Broadcast.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _worker;
        private readonly IGenericAttributeService _genericAttribute;

        public UserService(IUnitOfWork worker, IGenericAttributeService genericAttribute)
        {
            _worker = worker;
            _genericAttribute = genericAttribute;
        }

        #region User

        public async Task<User> InsertUserAsync(UserDto dto)
        {
            var userRepo = _worker.GetRepositoryAsync<User>();
            var roleRepo = _worker.GetRepositoryAsync<Role>();

            var user = new User
            {
                Guid = Guid.NewGuid(),
                Name = dto.FullName,
                GivenName = dto.FullName,
                AccountName = dto.Username,
                Email = dto.Email ?? string.Empty,
                PhoneNumber = dto.Phone ?? string.Empty,
                Title = dto.Designation ?? string.Empty,
                Department = dto.Department ?? string.Empty,
            };

            //handle password
            await _genericAttribute.SaveAttributeAsync(user, UserDefaults.PasswordAttribute, dto.Password);

            //handle roles
            if (dto.Roles?.Length > 0)
            {
                foreach (var r in dto.Roles)
                {
                    var role = await roleRepo.SingleAsync(x => x.Name == r);
                    if (role != null) user.AddUserRole(new UserRole { Role = role });
                }
            }

            await userRepo.AddAsync(user);
            await _worker.SaveChangesAsync();

            return user;
        }

        #endregion

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