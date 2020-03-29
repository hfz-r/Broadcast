using System;
using Broadcast.Core.Domain.Common;
using Broadcast.Core.Infrastructure;
using Broadcast.Services.Common;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test.Common
{
    public class GenericAttributeServiceTests
    {
        private IGenericAttributeService _genericAttributeService;
        private Mock<IUnitOfWork> _worker;

        [SetUp]
        public void SetUp()
        {
            _worker = new Mock<IUnitOfWork>();

            _genericAttributeService = new GenericAttributeService(_worker.Object);
        }

        public void Should_set_createdOrUpdatedDateUtc_in_InsertAttribute()
        {
            var attribute = new GenericAttribute
            {
                Key = "test",
                CreatedOrUpdatedDateUtc = null
            };

            _genericAttributeService.InsertAttributeAsync(attribute);

            Assert.That(attribute.CreatedOrUpdatedDateUtc, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void Should_update_createdOrUpdatedDateUtc_in_UpdateAttribute()
        {
            var attribute = new GenericAttribute
            {
                Key = "test",
                CreatedOrUpdatedDateUtc = DateTime.UtcNow.AddDays(-30)
            };

            _genericAttributeService.UpdateAttributeAsync(attribute);

            Assert.That(attribute.CreatedOrUpdatedDateUtc, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
}