using System.Threading.Tasks;
using Broadcast.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Broadcast.Infrastructure.Auth.Policies
{
    public class SignedInRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsValid(IAuthenticationService authentication)
        {
            return await authentication.AuthenticatedUserAsync() != null;
        }
    }
}