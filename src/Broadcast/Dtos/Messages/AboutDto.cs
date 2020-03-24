using System;
using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class AboutDto
    {
        [JsonProperty("category")]
        public string[] Categories { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}