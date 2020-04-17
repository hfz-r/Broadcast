using Broadcast.Core.Domain.Users;

namespace Broadcast.Infrastructure
{
    public interface ICurrentUserAccessor
    {
        User CurrentUser { get; set; }

        string GetCurrentUsername();
    }
}