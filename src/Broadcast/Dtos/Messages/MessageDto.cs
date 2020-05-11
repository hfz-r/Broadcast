using System;
using Broadcast.Dtos.Projects;
using Broadcast.Dtos.Users;
using Newtonsoft.Json;

namespace Broadcast.Dtos.Messages
{
    public class MessageDto
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("projectSelector")]
        public ProjectDto ProjectDto { get; set; }

        [JsonProperty("author")]
        public UserDto AuthorDto { get; set; }

        [JsonProperty("projectAbout")]
        public AboutDto AboutDto { get; set; }

        [JsonProperty("projectDetails")]
        public DetailsDto DetailsDto { get; set; }

        [JsonProperty("projectExtras")]
        public ExtrasDto ExtrasDto { get; set; }

        [JsonProperty("projectPreferences")]
        public PreferencesDto PreferencesDto { get; set; }
    }
}