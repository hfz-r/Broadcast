using Newtonsoft.Json;
using System;
using Broadcast.Core.Dtos.Messages;

namespace Broadcast.Core.Dtos.Projects
{
    public class ProjectDto
    {
        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("created_on")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("modified_on")]
        public DateTime? ModifiedOn { get; set; }

        [JsonProperty("modified_by")]
        public string ModifiedBy { get; set; }

        [JsonProperty("message_ids")]
        public int[] MessageIds { get; set; }
    }
}