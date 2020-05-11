using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Core.Domain.Projects
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public string Modifiedby { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
   
}
