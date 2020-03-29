using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Messages
{
    [Route("api/messages")]
    public class MessagesController : Controller
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<MessagesEnvelope> Get([FromQuery] string tag, [FromQuery] string author, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(tag, author, limit, offset));
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<MessageEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}