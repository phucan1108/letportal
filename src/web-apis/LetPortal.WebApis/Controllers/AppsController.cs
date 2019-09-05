using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Commands;
using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Models.Apps;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/apps")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger _logger;

        public AppsController(IMediator mediator, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<AppsController>();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App), 200)]
        public async Task<IActionResult> GetOne(string id)
        {
            var result = await _mediator.Send(new GetAppRequest(new Portal.Handlers.Apps.Queries.GetAppQuery { AppId = id }));

            return Ok(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<App>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] string ids)
        {
            var result = await _mediator.Send(new GetAppsRequest(new Portal.Handlers.Apps.Queries.GetAppsQuery { AppIds = ids.Split(";").ToList() }));
            return Ok(result);
        }

        [HttpGet("{id}/urls")]
        [ProducesResponseType(typeof(List<AvailableUrl>), 200)]
        public async Task<IActionResult> GetAvailableUrls(string id)
        {
            var result = await _mediator.Send(new GetAvailableUrlsRequest(new Portal.Handlers.Apps.Queries.GetAvailableUrlsQuery { AppId = id }));
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(App), 200)]
        public async Task<IActionResult> Create([FromBody] CreateAppCommand createAppCommand)
        {
            var result = await _mediator.Send(new CreateAppRequest(createAppCommand));

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateAppCommand updateAppCommand)
        {
            updateAppCommand.AppId = id;

            await _mediator.Send(new UpdateAppRequest(updateAppCommand));
            return Ok();
        }

        [HttpPut("{id}/menus")]
        public async Task<IActionResult> UpdateMenu(string id, [FromBody] UpdateAppMenuCommand updateAppMenuCommand)
        {
            updateAppMenuCommand.AppId = id;

            await _mediator.Send(new UpdateAppMenuRequest(updateAppMenuCommand));
            return Ok();
        }

        [HttpPost("menus/assign-role")]
        public async Task<IActionResult> AsssignRolesToMenu([FromBody] AssignRoleToMenuCommand assignRoleToMenuCommand)
        {
            await _mediator.Send(new AssignRoleToMenuRequest(assignRoleToMenuCommand));

            return Ok();
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<App>), 200)]
        public async Task<IActionResult> GetAllApps()
        {
            var result = await _mediator.Send(new GetAllAvailableAppsRequest(new Portal.Handlers.Apps.Queries.GetAllAvailableAppsQuery()));

            return Ok(result);
        }
    }
}
