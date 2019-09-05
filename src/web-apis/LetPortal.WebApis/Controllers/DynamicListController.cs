using LetPortal.Core.Logger;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.DynamicLists.Commands;
using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Models.DynamicLists;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/dynamiclists")]
    [ApiController]
    public class DynamicListController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IServiceLogger<DynamicListController> _serviceLogger;

        public DynamicListController(IMediator mediator, IServiceLogger<DynamicListController> serviceLogger)
        {
            _mediator = mediator;
            _serviceLogger = serviceLogger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<DynamicList>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllDynamicListRequest(new GetAllDynamicListQuery())));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DynamicList), 200)]
        public async Task<IActionResult> GetOne(string id)
        {
            return Ok(await _mediator.Send(new GetOneDynamicListRequest(new GetOneDynamicListQuery { DynamicListId = id })));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Create([FromBody] CreateDynamicListCommand createDynamicListCommand)
        {
            return Ok(await _mediator.Send(new CreateDynamicListRequest(createDynamicListCommand)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateDynamicListCommand updateDynamicListCommand)
        {
            updateDynamicListCommand.DynamicListId = id;
            await _mediator.Send(new UpdateDynamicLIstRequest(updateDynamicListCommand));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteDynamicListRequest(new DeleteDynamicListCommand { Id = id }));
            return Ok();
        }

        [HttpPost("fetch-data")]
        [ProducesResponseType(typeof(DynamicListResponseDataModel), 200)]
        public async Task<IActionResult> ExecuteQuery([FromBody] FetchingDataForDynamicListQuery fetchingDataForDynamicListQuery)
        {
            return Ok(await _mediator.Send(new FetchingDataForDynamicListRequest(fetchingDataForDynamicListQuery)));
        }

        [HttpPost("extract-query")]
        [ProducesResponseType(typeof(PopulateQueryModel), 200)]
        public async Task<IActionResult> ExtractingQuery([FromBody] ExtractingQueryForDynamicListQuery extractingQueryForDynamicListQuery)
        {
            return Ok(await _mediator.Send(new ExtractingQueryForDynamicListRequest(extractingQueryForDynamicListQuery)));
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _mediator.Send(new CheckNameIsExistRequest(new CheckNameIsExistQuery { Name = name })));
        }
    }
}
