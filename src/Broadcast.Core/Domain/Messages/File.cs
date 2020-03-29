namespace Broadcast.Core.Domain.Messages
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public int FileSize { get; set; }

        public byte[] FileContent { get; set; }

        public int MessageId { get; set; }

        public virtual Message Message { get; set; }
    }
}