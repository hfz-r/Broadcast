using System.Reflection;
using Broadcast.Core.Infrastructure;
using EasyCaching.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class CommonStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //mediatr
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //easy-cache
            services.AddEasyCaching();
            //dist mem-cache
            services.AddDistributedMemoryCache();
            //swagger
            services.AddSwaggerGen();
            //client handler
            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp/build");
        }

        public void Configure(IApplicationBuilder application)
        {
            //client handler
            application.UseDefaultFiles();
            application.UseStaticFiles();
            application.UseSpaStaticFiles();
            //swagger
            application.UseSwagger();
            //easy-cache
            application.UseEasyCaching();
        }

        public int Order => 100;
    }
}