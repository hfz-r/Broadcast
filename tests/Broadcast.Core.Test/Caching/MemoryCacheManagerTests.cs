using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Core.Caching;
using EasyCaching.InMemory;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Core.Test.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        private MemoryCacheManager _cacheManager;

        [SetUp]
        public void Setup()
        {
            _cacheManager = new MemoryCacheManager(new DefaultInMemoryCachingProvider("brdcst.tests",
                new List<IInMemoryCaching> {new InMemoryCaching("brdcst.tests", new InMemoryCachingOptions())},
                new InMemoryOptions()));
        }

        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            _cacheManager.SetAsync("some_key_1", 3, int.MaxValue).GetAwaiter().GetResult();
            _cacheManager.GetAsync("some_key_1", () => Task.FromResult(0)).Result.Should().Be(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            _cacheManager.SetAsync("some_key_1", 3, int.MaxValue).GetAwaiter().GetResult();
            _cacheManager.SetAsync("some_key_2", 4, int.MaxValue).GetAwaiter().GetResult();

            _cacheManager.IsSetAsync("some_key_1").Result.Should().BeTrue();
            _cacheManager.IsSetAsync("some_key_3").Result.Should().BeFalse();
        }

        [Test]
        public void Can_clear_cache()
        {
            _cacheManager.SetAsync("some_key_1", 3, int.MaxValue).GetAwaiter().GetResult();

            _cacheManager.ClearAsync().GetAwaiter().GetResult();

            _cacheManager.IsSetAsync("some_key_1").Result.Should().BeFalse();
        }

        [Test]
        public void Can_perform_lock()
        {
            const string key = "brdcst.task";
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                _cacheManager.IsSetAsync(key).Result.Should().BeTrue();

                _cacheManager.PerformActionWithLockAsync(key, expiration,
                    () => Assert.Fail("Action in progress")).Result.Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            _cacheManager.PerformActionWithLockAsync(key, expiration, action).Result.Should().BeTrue();
            actionCount.Should().Be(1);

            _cacheManager.Awaiting(a => a.PerformActionWithLockAsync(key, expiration, action)).Should()
                .Throw<ApplicationException>();
            actionCount.Should().Be(2);

            _cacheManager.PerformActionWithLockAsync(key, expiration, action).Result.Should().BeTrue();
            actionCount.Should().Be(3);
        }
    }
}