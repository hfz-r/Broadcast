using System.Collections.Generic;

namespace Broadcast.Domain
{
    public class Tag : BaseEntity
    {
        public string TagName { get; set; }

        public virtual ICollection<MessageTag> MessageTags { get; set; }
    }
}