using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;

namespace Broadcast.Services.Users
{
    public interface IUserService
    {
        Task<Role> GetRoleByNameAsync(string name);
        Task InsertRoleAsync(Role role);
    }
}