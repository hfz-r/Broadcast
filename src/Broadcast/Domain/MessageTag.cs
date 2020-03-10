namespace Broadcast.Domain
{
    public class MessageTag : BaseEntity
    {
        public int MessageId { get; set; }

        public virtual Message Message { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
