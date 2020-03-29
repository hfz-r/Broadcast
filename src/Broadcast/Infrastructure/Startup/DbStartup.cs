using Broadcast.Core.Infrastructure;
using Broadcast.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class DbStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 10;
    }
}