using Broadcast.Dtos.Projects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcast.Features.Project
{
    public class Project
    {
        public class ProjectEnvelope
        {
            public ProjectEnvelope(ProjectDto project)
            {
                Project = project;
            }

            [JsonProperty("project")]
            public ProjectDto Project { get; }
        }

        public class ProjectsEnvelope
        {
            [JsonProperty("projects")]
            public List<ProjectDto> Projects { get; set; }

            [JsonProperty("projectCount")]
            public int ProjectsCount { get; set; }
        }
    }
}
