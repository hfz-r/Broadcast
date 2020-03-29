using System;
using System.Threading.Tasks;

namespace Broadcast.Core.Caching
{
    public interface ILocker
    {
        /// <summary>
        /// Perform action with exclusive in-memory lock
        /// </summary>
        Task<bool> PerformActionWithLockAsync(string key, TimeSpan expirationTime, Action action);
    }
}