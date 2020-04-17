using Broadcast.Core.Infrastructure;
using Broadcast.Infrastructure.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class AuthStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationPipeline();
            services.AddAuthorizationPipeline();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseAuthentication();
        }

        public int Order => 500;
    }
}