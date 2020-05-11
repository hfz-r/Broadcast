using Newtonsoft.Json;
using System;

namespace Broadcast.Dtos.Projects
{
    public class ProjectDto
    {
        [JsonProperty("project")]
        public string Project { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdon")]
        public DateTime CreatedOn { get; set; }


        [JsonProperty("createdby")]
        public string CreatedBy { get; set; }

        [JsonProperty("modifiedon")]
        public DateTime ModifiedOn { get; set; }

        [JsonProperty("modifiedby")]
        public string ModifiedBy { get; set; }
    }
}