using System;
using System.Collections.Generic;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Mvc;
using Broadcast.Services.Logging;
using EasyCaching.Core;
using EasyCaching.InMemory;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Broadcast.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            services.AddHttpContextAccessor();

            var engine = EngineContext.Create();
            var serviceProvider = engine.ConfigureServices(services, configuration);

            engine.Resolve<ILogger>().InformationAsync("Application started");

            return serviceProvider;
        }

        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(
                    new Dictionary<string, IEnumerable<string>> {{"Bearer", new string[] { }}});
                options.SwaggerDoc("v1", new Info {Title = "Hartalega Broadcast API", Version = "v1"});
                options.CustomSchemaIds(y => y.FullName);
                options.DocInclusionPredicate((version, apiDescription) => true);
                options.TagActionsBy(y => new List<string> {y.GroupName});
            });
        }

        public static void AddEasyCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(option => { option.UseInMemory(); });
        }

        public static IMvcBuilder AddMvcPipeline(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();

            mvcBuilder.AddMvcOptions(options => options.Conventions.Add(new GroupByApiRootConvention()));
            mvcBuilder.AddMvcOptions(options => options.Filters.Add<ValidatorActionFilter>());

            mvcBuilder.AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            mvcBuilder.AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<Broadcast.Startup>();
                config.ImplicitlyValidateChildProperties = true;
            });

            return mvcBuilder;
        }
    }
}