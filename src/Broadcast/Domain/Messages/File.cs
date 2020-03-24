namespace Broadcast.Domain.Messages
{
    public class File : BaseEntity
    {
        public int MessageId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public int FileSize { get; set; }

        public byte[] FileContent { get; set; }

        public virtual Message Message { get; set; }
    }
}