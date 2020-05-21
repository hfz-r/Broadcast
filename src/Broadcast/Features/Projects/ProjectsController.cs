using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Features.Projects
{
    [Route("api/projects")]
    [Authorize(Policy = "DefaultPolicy")]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ProjectsEnvelope> Get([FromQuery] string name, [FromQuery] int? size, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(name, size, offset));
        }

        [HttpPost]
        public async Task<ProjectEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{slug}")]
        //[HasPermission()]
        public async Task<ProjectEnvelope> Edit(string slug, [FromBody] Edit.Command command)
        {
            command.Slug = slug;
            return await _mediator.Send(command);
        }
    }
}