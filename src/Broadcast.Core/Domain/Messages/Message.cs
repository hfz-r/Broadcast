using System;
using System.Collections.Generic;
using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Domain.Users;
using Newtonsoft.Json;

namespace Broadcast.Core.Domain.Messages
{
    public class Message : BaseEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Slug { get; set; }

        public virtual Project Project { get; set; }

        public virtual User Author { get; set; }

        public virtual Preference Preference { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<MessageTag> MessageTags { get; set; }

        [JsonIgnore]
        public virtual ICollection<MessageCategory> MessageCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<File> Files { get; set; }

        #region Methods

        public void AddMessageTag(MessageTag messageTag) => MessageTags.Add(messageTag);
        public void RemoveMessageTag(MessageTag messageTag) => MessageTags.Remove(messageTag);

        public void AddMessageCategory(MessageCategory messageCategory) => MessageCategories.Add(messageCategory);
        public void RemoveMessageCategory(MessageCategory messageCategory) => MessageCategories.Remove(messageCategory);

        #endregion
    }
}