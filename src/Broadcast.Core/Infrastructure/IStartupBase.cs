using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Core.Infrastructure
{
    public interface IStartupBase
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        void Configure(IApplicationBuilder application);

        int Order { get; }
    }
}