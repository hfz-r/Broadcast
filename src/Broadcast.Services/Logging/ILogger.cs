using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Logging;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure.Paging;

namespace Broadcast.Services.Logging
{
    public interface ILogger
    {
        Task DeleteLog(Log log);
        Task DeleteLogs(IList<Log> logs);
        Task ErrorAsync(string message, Exception exception = null, User user = null);
        Task<IPaginate<Log>> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null, string message = "", LogLevel? logLevel = null, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<Log> GetLogById(int logId);
        Task<IList<Log>> GetLogByIds(int[] logIds);
        Task InformationAsync(string message, Exception exception = null, User user = null);
        Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null);
        bool IsEnabled(LogLevel level);
        Task WarningAsync(string message, Exception exception = null, User user = null);
    }
}