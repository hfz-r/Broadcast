using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Broadcast.Services.Auth
{
    public static class AccountManagementExtensions
    {
        public static T GetProperty<T>(this Principal principal, string property)
        {
            return principal.GetUnderlyingObject() is DirectoryEntry directoryEntry &&
                   directoryEntry.Properties.Contains(property)
                ? (T) directoryEntry.Properties[property].Value
                : default(T);
        }
    }
}