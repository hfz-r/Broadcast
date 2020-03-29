using System.Collections.Generic;
using Broadcast.Core.Domain.Messages;

namespace Broadcast.Core.Domain.Tags
{
    public class Tag : BaseEntity
    {
        public string TagName { get; set; }

        public virtual ICollection<MessageTag> MessageTags { get; set; }
    }
}