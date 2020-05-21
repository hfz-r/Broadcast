using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Dtos.Users;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Infrastructure.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Users
{
    [Route("api/users")]
    [Authorize(Policy = "DefaultPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserAccessor _currentUser;

        public UsersController(IMediator mediator, ICurrentUserAccessor currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("current")]
        //[HasPermission(StandardPermission.UserRead)]
        public async Task<UserEnvelope> GetCurrentUser()
        {
            return await Task.FromResult(new UserEnvelope(_currentUser.CurrentUser.ToDto<UserDto>()));
        }

        [HttpGet]
        //[HasPermission(StandardPermission.UserRead)]
        public async Task<UsersEnvelope> Get([FromQuery] string department, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(department, limit, offset));
        }

        [HttpPut("{username}")]
        //[HasPermission(StandardPermission.UserChange)]
        public async Task<UserEnvelope> Edit(string username, [FromBody] Edit.Command command)
        {
            command.Username = username;
            return await _mediator.Send(command);
        }
    }
}