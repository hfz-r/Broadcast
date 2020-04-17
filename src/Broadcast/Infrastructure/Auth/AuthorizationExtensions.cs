using Broadcast.Infrastructure.Auth.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Auth
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorizationPipeline(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.Requirements.Add(new AuthorizationSchemeRequirement());
                    policy.Requirements.Add(new SignedInRequirement());
                    policy.RequireAuthenticatedUser();
                });
            });
        }
    }
}