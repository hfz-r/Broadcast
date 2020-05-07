using Broadcast.Core.Domain.Users;

namespace Broadcast.Core
{
    public interface ICurrentUserAccessor
    {
        User CurrentUser { get; }

        string GetCurrentUsername();
    }
}