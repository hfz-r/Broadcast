using System;
using Broadcast.Core.Infrastructure;
using Broadcast.Test;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public virtual void SetUp()
        {
        }

        public void RunWithTestServiceProvider(Action action)
        {
            var engine = new Mock<Engine>();
            engine.Setup(x => x.ServiceProvider).Returns(new TestServiceProvider());
            EngineContext.Replace(engine.Object);

            action();

            EngineContext.Replace(null);
        }
    }
}