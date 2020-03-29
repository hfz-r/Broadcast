using System;
using Broadcast.Core.Domain.Users;

namespace Broadcast.Core.Domain.Logging
{
    public class Log : BaseEntity
    {
        public int LogLevelId { get; set; }

        public string ShortMessage { get; set; }

        public string FullMessage { get; set; }

        public int? UserId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public LogLevel LogLevel
        {
            get => (LogLevel)LogLevelId;
            set => LogLevelId = (int)value;
        }

        public virtual User User { get; set; }
    }
}