using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Core.Infrastructure.TypeFinder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Core.Infrastructure
{
    public class Engine : IEngine
    {
        private IServiceProvider _serviceProvider { get; set; }

        #region Utilities

        protected IServiceProvider GetServiceProvider()
        {
            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            var context = accessor?.HttpContext;
            return context?.RequestServices ?? ServiceProvider;
        }

        protected void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
        {
            var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile) Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            AutoMapperConfiguration.Init(config);
        }

        protected virtual IServiceProvider RegisterDependencies(IServiceCollection services, ITypeFinder typeFinder)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            containerBuilder.Populate(services);

            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            var instances = dependencyRegistrars
                .Select(dependencyRegistrar => (IDependencyRegistrar) Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder);

            _serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

            return _serviceProvider;
        }

        #endregion

        public IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var typeFinder = new AppDomainTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartupBase>();

            var instances = startupConfigurations.Select(startup => (IStartupBase) Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);

            AddAutoMapper(services, typeFinder);

            RegisterDependencies(services, typeFinder);

            return _serviceProvider;
        }

        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            var typeFinder = Resolve<ITypeFinder>();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartupBase>();

            var instances = startupConfigurations
                .Select(startup => (IStartupBase) Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            foreach (var instance in instances)
                instance.Configure(application);
        }

        public T Resolve<T>() where T : class
        {
            return (T) Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            return GetServiceProvider()?.GetService(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>) GetServiceProvider().GetServices(typeof(T));
        }

        public object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Exception("Unknown dependency");
                        return service;
                    });

                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }

        #region Properties

        public IServiceProvider ServiceProvider => _serviceProvider;

        #endregion
    }
}