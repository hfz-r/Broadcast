using Broadcast.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class AuthenticationStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationPipeline();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseAuthentication();
        }

        public int Order => 500;
    }
}