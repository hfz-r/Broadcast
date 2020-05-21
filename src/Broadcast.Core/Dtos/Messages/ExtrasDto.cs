using Newtonsoft.Json;

namespace Broadcast.Core.Dtos.Messages
{
    public class ExtrasDto
    {
        [JsonProperty("files")]
        public FilesDto[] ExtraFiles { get; set; }
    }

    public class FilesDto
    {
        [JsonProperty("file_id")]
        public int FileId { get; set; }

        [JsonProperty("name")]
        public string FileName { get; set; }

        [JsonProperty("type")]
        public string FileType { get; set; }

        [JsonProperty("size")]
        public int FileSize { get; set; }

        [JsonProperty("data")]
        public string FileContent { get; set; }
    }
}