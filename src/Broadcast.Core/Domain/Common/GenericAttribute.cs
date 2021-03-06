﻿using System;

namespace Broadcast.Core.Domain.Common
{
    public class GenericAttribute : BaseEntity
    {
        public int EntityId { get; set; }

        public string KeyGroup { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedOrUpdatedDateUtc { get; set; }
    }
}
