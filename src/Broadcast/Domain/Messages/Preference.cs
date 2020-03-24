namespace Broadcast.Domain.Messages
{
    // todo
    public class Preference : BaseEntity
    {
        public int MessageId { get; set; }

        public bool? Cb1 { get; set; }

        public bool? Cb2 { get; set; }

        public virtual Message Message { get; set; }
    }
}