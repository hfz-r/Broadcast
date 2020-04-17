using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Broadcast.Services.Auth
{
    public static class AccountManagementExtensions
    {
        public static string GetProperty(this Principal principal, string property)
        {
            return principal.GetUnderlyingObject() is DirectoryEntry directoryEntry &&
                   directoryEntry.Properties.Contains(property)
                ? directoryEntry.Properties[property].Value.ToString()
                : string.Empty;
        }
    }
}