using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Dtos.Users;

namespace Broadcast.Services.Users
{
    public interface IUserService
    {
        Task<User> InsertUserAsync(UserDto dto);
        Task<Role> GetRoleByNameAsync(string name);
        Task InsertRoleAsync(Role role);
    }
}