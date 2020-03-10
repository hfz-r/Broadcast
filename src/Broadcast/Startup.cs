using System.Reflection;
using AutoMapper;
using Broadcast.Infrastructure;
using Broadcast.Infrastructure.Behaviors;
using Broadcast.Infrastructure.Errors;
using Broadcast.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Broadcast
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionBehavior<,>));

            services.AddDbContext<ApplicationDbContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
                .AddUnitOfWork<ApplicationDbContext>();

            services.AddLocalization(options => options.ResourcesPath = "Resources"); //todo
            services.AddSwaggerGen();

            services.AddCors();
            services.AddMvcPipeline();
            services.AddAutoMapper(GetType().Assembly);

            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp/build");

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthenticationPipeline();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Hartalega Broadcast V1"); });

            app.UseAuthentication();
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseReactDevelopmentServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
        }
    }
}