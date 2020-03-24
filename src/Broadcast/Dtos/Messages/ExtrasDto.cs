using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class ExtrasDto
    {
        [JsonProperty("files")]
        public FilesDto[] ExtraFiles { get; set; }
    }

    public class FilesDto
    {
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