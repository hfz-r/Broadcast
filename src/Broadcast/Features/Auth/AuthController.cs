using System;
using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Auth
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{guid}")]
        [Authorize(AuthenticationSchemes = "Authentication")]
        public async Task<TokenEnvelope> GenerateToken(string guid)
        {
            return await _mediator.Send(new Token.Query(guid));
        }

        [HttpPost]
        public async Task<Guid> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}