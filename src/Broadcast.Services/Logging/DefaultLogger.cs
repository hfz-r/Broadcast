using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Logging;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore;

namespace Broadcast.Services.Logging
{
    public class DefaultLogger : ILogger
    {
        private readonly IUnitOfWork _worker;

        public DefaultLogger(IUnitOfWork worker)
        {
            _worker = worker;
        }

        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        public async Task DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            var repo = _worker.GetRepositoryAsync<Log>();
            await repo.DeleteAsync(log);

            await _worker.SaveChangesAsync();
        }

        public async Task DeleteLogs(IList<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            var repo = _worker.GetRepositoryAsync<Log>();
            await repo.DeleteAsync(logs);

            await _worker.SaveChangesAsync();
        }

        public async Task<IPaginate<Log>> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LogLevel? logLevel = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var repo = _worker.GetRepositoryAsync<Log>();

            var logObj = await repo.GetQueryableAsync(queryExp: query =>
            {
                if (fromUtc.HasValue) query = query.Where(log => fromUtc.Value <= log.CreatedOnUtc);
                if (toUtc.HasValue) query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
                if (logLevel.HasValue) query = query.Where(l => (int) logLevel.Value == l.LogLevelId);
                if (!string.IsNullOrEmpty(message))
                    query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
                return query;
            }, orderBy: logs => logs.OrderByDescending(log => log.CreatedOnUtc));

            return await logObj.ToPaginateAsync(pageIndex, pageSize);
        }

        public async Task<Log> GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            var repo = _worker.GetRepositoryAsync<Log>();
            return await repo.SingleAsync(log => log.Id == logId);
        }

        public async Task<IList<Log>> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var repo = _worker.GetRepositoryAsync<Log>();
            var query = await repo.GetQueryableAsync(log => logIds.Contains(log.Id));

            var sortedLogItems = new List<Log>();
            foreach (var id in logIds)
            {
                var log = (await query.ToListAsync())?.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }

            return sortedLogItems;
        }

        public async Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "",
            User user = null)
        {
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                User = user,
                CreatedOnUtc = DateTime.UtcNow
            };

            var repo = _worker.GetRepositoryAsync<Log>();
            await repo.AddAsync(log);

            await _worker.SaveChangesAsync();

            return log;
        }

        public async Task InformationAsync(string message, Exception exception = null, User user = null)
        {
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Information))
                await InsertLogAsync(LogLevel.Information, message, exception?.ToString() ?? string.Empty, user);
        }

        public async Task WarningAsync(string message, Exception exception = null, User user = null)
        {
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Warning))
                await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, user);
        }

        public async Task ErrorAsync(string message, Exception exception = null, User user = null)
        {
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, user);
        }
    }
}