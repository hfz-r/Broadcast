using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Users;
using Broadcast.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Users
{
    [Route("api/users")]
    [Authorize(Policy = "DefaultPolicy")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserAccessor _currentUser;

        public UsersController(IMediator mediator, ICurrentUserAccessor currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("current")]
        public async Task<UserEnvelope> GetCurrentUser()
        {
            return await Task.FromResult(new UserEnvelope(_currentUser.CurrentUser.ToDto<UserDto>()));
        }
    }
}