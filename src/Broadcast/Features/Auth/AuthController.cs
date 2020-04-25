using System;
using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Auth
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAuthenticationService _authentication;

        public AuthController(IMediator mediator, IAuthenticationService authentication)
        {
            _mediator = mediator;
            _authentication = authentication;
        }

        [HttpGet("{guid}")]
        [Authorize(AuthenticationSchemes = "Authentication")]
        public async Task<TokenEnvelope> GenerateToken(string guid)
        {
            return await _mediator.Send(new Token.Query(guid));
        }

        [HttpGet("logout")]
        [Authorize(Policy = "DefaultPolicy")]
        public async Task Logout()
        {
            await _authentication.SignOutAsync();
        }

        [HttpPost]
        public async Task<Guid> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}