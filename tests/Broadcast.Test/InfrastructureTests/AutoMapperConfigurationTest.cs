using AutoMapper;
using Broadcast.Infrastructure.Mapper;
using NUnit.Framework;

namespace Broadcast.Test.InfrastructureTests
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(typeof(MapperProfile)); });

            AutoMapperConfiguration.Init(config);
            AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
        }
    }
}