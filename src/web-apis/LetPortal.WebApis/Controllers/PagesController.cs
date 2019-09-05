using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Handlers.Pages.Requests;
using LetPortal.Portal.Models.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Handlers.Pages.Commands;
using LetPortal.Portal.Handlers.Pages.Queries;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PagesController : ControllerBase
    {

        private readonly IMediator _mediator;

        private readonly ILogger _logger;

        public PagesController(IMediator mediator, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<PagesController>();
        }

        [HttpGet("shorts")]
        [ProducesResponseType(typeof(List<ShortPageModel>),200)]
        public async Task<IActionResult> GetAllShortPages()
        {
            var result = await _mediator.Send(new GetAllShortPagesRequest(new Portal.Handlers.Pages.Queries.GetAllShortPagesQuery()));
            return Ok(result);
        }

        [HttpGet("all-claims")]
        [ProducesResponseType(typeof(List<ShortPortalClaimModel>), 200)]
        public async Task<IActionResult> GetAllPortalClaims()
        {
            var result = await _mediator.Send(new GetAllPortalClaimsRequest(new Portal.Handlers.Pages.Queries.GetAllPortalClaimsQuery()));
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOneById(string id)
        {
            var result = await _mediator.Send(new GetOnePageByIdRequest(new Portal.Handlers.Pages.Queries.GetOnePageByIdQuery { PageId = id }));
            return Ok(result);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOne(string name)
        {
            var result = await _mediator.Send(new GetOnePageRequest(new Portal.Handlers.Pages.Queries.GetOnePageQuery { PageName = name }));
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Create([FromBody] CreatePageCommand createPageCommand)
        {
            return Ok(await _mediator.Send(new CreatePageRequest(createPageCommand)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePageCommand updatePageCommand)
        {
            updatePageCommand.PageId = id;
            await _mediator.Send(new UpdatePageRequest(updatePageCommand));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeletePageRequest(new DeletePageCommand { PageId = id }));
            return Ok();
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _mediator.Send(new CheckNameIsExistRequest(new CheckNameIsExistQuery { Name = name })));
        }
    }
}
