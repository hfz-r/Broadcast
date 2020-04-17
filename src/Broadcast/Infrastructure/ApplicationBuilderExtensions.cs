using Broadcast.Core.Infrastructure;
using Broadcast.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Broadcast.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        public static void UseSwagger(this IApplicationBuilder application)
        {
            application.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            application.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Hartalega Broadcast V1"); });
        }

        public static void UseErrorHandler(this IApplicationBuilder application)
        {
            application.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}