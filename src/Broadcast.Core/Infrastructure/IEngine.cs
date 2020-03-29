using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Core.Infrastructure
{
    public interface IEngine
    {
        IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration);

        void ConfigureRequestPipeline(IApplicationBuilder application);

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        IEnumerable<T> ResolveAll<T>();

        object ResolveUnregistered(Type type);
    }
}