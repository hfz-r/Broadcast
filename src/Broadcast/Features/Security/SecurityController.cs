using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Security
{
    [Route("api/security")]
    //[Authorize(Policy = "DefaultPolicy")]
    public class SecurityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SecurityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("roles")]
        //[HasPermission(StandardPermission.UserRead)]
        public async Task<SecuritiesEnvelope> GetRoles([FromQuery] string username, [FromQuery] int permissionId, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(username, permissionId, limit, offset));
        }
    }
}