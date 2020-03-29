using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Broadcast.Core.Domain.Logging;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Paging;
using Broadcast.Services.Logging;
using Broadcast.Test;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test.Logging
{
    [TestFixture]
    public class LoggingTests
    {
        private ILogger _logger;
        private Mock<DbContext> _dbContext;
        private Mock<IUnitOfWork> _worker;
        private Mock<IRepositoryAsync<Log>> _repository;
        private IList<Log> _logs;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new Mock<DbContext>();
            _worker = new Mock<IUnitOfWork>();
            _repository = new Mock<IRepositoryAsync<Log>>();
            _logs = new List<Log>();

            var log1 = new Log
            {
                Id = 1,
                ShortMessage = "Log number #1",
                FullMessage = "Long messages of log number #1",
                LogLevelId = 10,
                CreatedOnUtc = DateTime.UtcNow
            };
            var log2 = new Log
            {
                Id = 2,
                ShortMessage = "Log number #2",
                FullMessage = "Long messages of log number #2",
                LogLevelId = 20,
                CreatedOnUtc = DateTime.UtcNow
            };
            var log3 = new Log
            {
                Id = 3,
                ShortMessage = "Log number #3",
                FullMessage = "Long messages of log number #3",
                LogLevelId = 20,
                CreatedOnUtc = DateTime.UtcNow
            };

            _logs = new List<Log> {log1, log2, log3};
            var dataSet = _logs.BuildMockDbSet();

            _dbContext.Setup(x => x.Set<Log>()).Returns(dataSet.Object);

            _repository
                .Setup(x => x.GetPagedListAsync(null, null, null, 0, int.MaxValue, false, CancellationToken.None))
                .ReturnsAsync(dataSet.Object.ToPaginate(0, int.MaxValue));
            _worker.Setup(x => x.GetRepositoryAsync<Log>()).Returns(() => new RepositoryAsync<Log>(_dbContext.Object));

            _logger = new DefaultLogger(_worker.Object);
        }

        [Test]
        public void Should_return_all_provided_logs()
        {
            var result = _logger.GetAllLogs().Result;

            result.Count.Should().Be(3);
            result.Items.Count(log => log.LogLevel == LogLevel.Information).Should().Be(2);
        }

        [Test]
        public void Can_add_new_log()
        {
            //arrange - add
            _dbContext.Setup(x => x.Set<Log>().AddAsync(It.IsAny<Log>(), CancellationToken.None))
                .Callback((Log log, CancellationToken token) => _logs.Add(log));

            //act
            _logger.InsertLogAsync(LogLevel.Information, "Add new log", "Long messages of the new log");

            //arrange - get
            var dataSet = _logs.BuildMockDbSet();
            _dbContext.Setup(x => x.Set<Log>()).Returns(dataSet.Object);

            var result = _logger.GetAllLogs(logLevel: LogLevel.Information).Result;

            //assert
            result.Count.Should().Be(3);
            result.Items.Any(l => l.ShortMessage.Contains("new log")).Should().BeTrue();
        }

        [Test]
        public void Should_return_logs_by_ids()
        {
            var result = _logger.GetLogByIds(new[] {2, 3}).Result;

            result.Select(l => l.LogLevel).Should().AllBeEquivalentTo(LogLevel.Information);
        }
    }
}