using System.Threading.Tasks;
using Broadcast.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Broadcast.Infrastructure.Auth.Policies
{
    public class SignedInHandler : AuthorizationHandler<SignedInRequirement>
    {
        private readonly IAuthenticationService _authentication;

        public SignedInHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SignedInRequirement requirement)
        {
            if (await requirement.IsValid(_authentication))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}