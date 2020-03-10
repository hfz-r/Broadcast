using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Broadcast.Domain
{
    public class Message : BaseEntity
    {
        private IList<string> _tagList;

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public virtual User Author { get; set; }

        [NotMapped]
        public virtual IList<string> TagList => _tagList ?? (_tagList = MessageTags.Select(t => t.Tag.TagName).ToList());

        [JsonIgnore]
        public virtual ICollection<MessageTag> MessageTags { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}