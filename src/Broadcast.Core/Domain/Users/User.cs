using System;
using System.Collections.Generic;

namespace Broadcast.Core.Domain.Users
{
    public class User : BaseEntity
    {
        private ICollection<UserRole> _userRoles;

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

        public virtual ICollection<UserRole> UserRoles
        {
            get => _userRoles ?? (_userRoles = new List<UserRole>());
            protected set => _userRoles = value;
        }

        #region Methods

        public void AddUserRole(UserRole userRole) => UserRoles.Add(userRole);

        public void RemoveUserRole(UserRole userRole) => UserRoles.Remove(userRole);

        #endregion
    }
}