using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;

namespace Broadcast.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<User> SignInAsync(string adUsername, string adPassword);
        Task SignOutAsync();
        Task<User> AuthenticatedUserAsync();
    }
}