using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Infrastructure.Auth.Policies
{
    public class AuthorizationSchemeRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsValid(IHeaderDictionary requestHeaders)
        {
            return await Task.FromResult(
                requestHeaders != null &&
                requestHeaders.ContainsKey("Authorization") &&
                requestHeaders["Authorization"].ToString().Contains(JwtBearerDefaults.AuthenticationScheme));
        }
    }
}