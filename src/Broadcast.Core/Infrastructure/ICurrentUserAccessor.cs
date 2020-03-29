using Broadcast.Core.Domain.Users;

namespace Broadcast.Core.Infrastructure
{
    public interface ICurrentUserAccessor
    {
        User CurrentUser { get; set; }

        string GetCurrentUsername();
    }
}