using Newtonsoft.Json;
using System.Collections.Generic;
using Broadcast.Core.Dtos.Projects;

namespace Broadcast.Features.Projects
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