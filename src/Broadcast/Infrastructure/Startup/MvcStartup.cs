using Broadcast.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Startup
{
    public class MvcStartup : IStartupBase
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //cors
            services.AddCors();
            //mvc
            services.AddMvcPipeline();
        }

        public void Configure(IApplicationBuilder application)
        {
            //cors
            application.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            //mvc
            application.UseMvc();
            //spa client
            application.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (EngineContext.Current.Resolve<IHostingEnvironment>().IsDevelopment())
                {
                    //spa.UseReactDevelopmentServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3003");
                }
            });
        }

        public int Order => 1000;
    }
}