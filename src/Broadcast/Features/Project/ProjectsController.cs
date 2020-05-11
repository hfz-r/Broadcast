using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Broadcast.Features.Project.Project;

namespace Broadcast.Features.Project
{
    [Route("api/[controller]")]
    [Authorize(Policy = "DefaultPolicy")]
    public class ProjectsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpGet]
        public async Task<ProjectsEnvelope> Get([FromQuery] string name, [FromQuery] int size)
        {
            return await _mediator.Send(new List.Query(name, size));
        }


        [HttpPost]
        public async Task<ProjectEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}