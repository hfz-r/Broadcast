using System.ComponentModel.DataAnnotations;

namespace Broadcast.Core.Domain.Security
{
    public enum Permission1
    {
        [Display(Name = "NotSet", Description = "Error")]
        NotSet = 0,

        [Display(GroupName = "Widget", Name = "WidgetRead", Description = "Can view widget content")]
        WidgetRead = 10,

        [Display(GroupName = "Announcement", Name = "AnnouncementRead", Description = "Can view announcement content")]
        AnnouncementRead = 20,
        [Display(GroupName = "Announcement", Name = "AnnouncementChange", Description = "Can create, update or delete announcement")]
        AnnouncementChange = 21,

        [Display(GroupName = "UserManagement", Name = "UserRead", Description = "Can view user content")]
        UserRead = 30,
        [Display(GroupName = "UserManagement", Name = "UserChange", Description = "Can create, update or delete user")]
        UserChange = 31,

        [Display(GroupName = "Admin", Name = "AccessAll", Description = "Allows access to every feature")]
        AccessAll = short.MaxValue,
    }
}