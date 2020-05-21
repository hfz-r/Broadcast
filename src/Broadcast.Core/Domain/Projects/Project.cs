using System;
using System.Collections.Generic;
using Broadcast.Core.Domain.Messages;

namespace Broadcast.Core.Domain.Projects
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}