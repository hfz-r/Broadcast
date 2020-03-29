using Broadcast.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class ErrorHandlerStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseErrorHandler();
        }

        public int Order => 0;
    }
}
