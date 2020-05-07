using System.Collections.Generic;
using Broadcast.Core.Domain.Security;
using Broadcast.Core.Domain.Users;

namespace Broadcast.Core.Infrastructure.Security
{
    public enum StandardPermission
    {
        WidgetRead = 10,
        AnnouncementRead = 20,
        AnnouncementChange = 21,
        UserRead = 30,
        UserChange = 31,
    }

    public class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly Permission WidgetRead = new Permission {Name = "WidgetRead", Category = "Widget"};

        public static readonly Permission AnnouncementRead = new Permission {Name = "AnnouncementRead", Category = "Announcements"};
        public static readonly Permission AnnouncementChange = new Permission {Name = "AnnouncementChange", Category = "Announcements"};

        public static readonly Permission UserRead = new Permission {Name = "UserRead", Category = "Managements"};
        public static readonly Permission UserChange = new Permission {Name = "UserChange", Category = "Managements"};

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {WidgetRead, AnnouncementRead, AnnouncementChange, UserRead, UserChange};
        }

        public HashSet<(string roleName, Permission[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string, Permission[])>
                {
                    (
                    UserDefaults.AdminRole,
                    new[] {AnnouncementRead, AnnouncementChange, UserRead, UserChange, WidgetRead}
                    ),
                    (
                    UserDefaults.TesterRole,
                    new[] {AnnouncementRead, UserRead}
                    )
                };
        }
    }
}