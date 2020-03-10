using System.Threading.Tasks;
using MediatR;
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
    }
}