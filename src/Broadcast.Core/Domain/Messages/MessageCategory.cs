namespace Broadcast.Core.Domain.Messages
{
    public class MessageCategory : BaseEntity
    {
        public int MessageId { get; set; }

        public virtual Message Message { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }  
    }
}