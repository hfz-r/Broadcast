using System.Collections.Generic;
using Broadcast.Domain.Messages;

namespace Broadcast.Domain.Tags
{
    public class Tag : BaseEntity
    {
        public string TagName { get; set; }

        public virtual ICollection<MessageTag> MessageTags { get; set; }
    }
}