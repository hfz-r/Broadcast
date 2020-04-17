using System;

namespace Broadcast.Core.Domain.Users
{
    public class User : BaseEntity
    {
        public Guid Guid { get; set; }

        public string AccountName { get; set; }

        public string PrincipalName { get; set; }

        public string Name { get; set; }

        public string GivenName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public byte[] Photo { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        public string Title { get; set; }
    }
}