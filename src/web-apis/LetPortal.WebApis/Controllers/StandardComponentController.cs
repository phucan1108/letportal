using LetPortal.Core.Logger;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.Standards.Commands;
using LetPortal.Portal.Handlers.Components.Standards.Queries;
using LetPortal.Portal.Handlers.Components.Standards.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/standards")]
    [ApiController]
    public class StandardComponentController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IServiceLogger<StandardComponentController> _serviceLogger;

        public StandardComponentController(
            IMediator mediator, 
            IServiceLogger<StandardComponentController> serviceLogger)
        {
            _serviceLogger = serviceLogger;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StandardComponent), 200)]
        public async Task<IActionResult> GetOne(string id)
        {
            return Ok(await _mediator.Send(new GetOneStandardComponentRequest(new Portal.Handlers.Components.Standards.Queries.GetOneStandardComponentQuery { StandardId = id })));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> CreateOne([FromBody] CreateStandardComponentCommand createStandardComponentCommand)
        {
            return Ok(await _mediator.Send(new CreateStandardComponentRequest(createStandardComponentCommand)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOne(string id, [FromBody] UpdateStandardComponentCommand updateStandardComponentCommand)
        {
            updateStandardComponentCommand.StandardId = id;
            return Ok(await _mediator.Send(new UpdateStandardComponentRequest(updateStandardComponentCommand)));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] CreateBulkStandardComponentsCommand createBulkStandardComponentsCommand)
        {
            await _mediator.Send(new CreateBulkStandardComponentsRequest(createBulkStandardComponentsCommand));
            return Ok();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteBulk([FromQuery] string ids)
        {
            await _mediator.Send(new DeleteBulkStandardComponentsRequest(new DeleteBulkStandardComponentsCommand { StandardIds = ids.Split(";").ToList() }));
            return Ok();
        }

        [HttpGet("bulk")]
        [ProducesResponseType(typeof(List<StandardComponent>), 200)]
        public async Task<IActionResult> GetManys([FromQuery] string ids)
        {
            return Ok(await _mediator.Send(new GetAllStandardComponentsByIdsRequest(new Portal.Handlers.Components.Standards.Queries.GetAllStandardComponentsByIdsQuery { Ids = ids?.Split(";").ToList() })));
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _mediator.Send(new CheckNameIsExistRequest(new CheckNameIsExistQuery { Name = name })));
        }
    }
}
