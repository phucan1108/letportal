using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Charts;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Services.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/charts")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly IChartRepository _chartRepository;

        private readonly IChartService _chartService;

        private readonly IServiceLogger<ChartsController> _logger;

        public ChartsController(
            IChartRepository chartRepository,
            IChartService chartService,
            IServiceLogger<ChartsController> logger)
        {
            _chartService = chartService;
            _chartRepository = chartRepository;
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<Chart>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetMany()
        {
            return Ok(await _chartRepository.GetAllAsync(isRequiredDiscriminator: true));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Chart), 200)]
        [Authorize]
        public async Task<IActionResult> GetOne(string id)
        {
            _logger.Info("Getting Chart with Id = {id}", id);
            var result = await _chartRepository.GetOneAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            // Hide infos when datasource is database
            result.DatabaseOptions.Query =
                string.Join(';',
                    StringUtil
                        .GetAllDoubleCurlyBraces(
                            result.DatabaseOptions.Query, true,
                                new List<string> { "{{REAL_TIME}}", "{{FILTER}}" }));
            _logger.Info("Found chart: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{id}/builder")]
        [ProducesResponseType(typeof(Chart), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetOneForBuilder(string id)
        {
            _logger.Info("Getting Chart with Id = {id}", id);
            var result = await _chartRepository.GetOneAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            _logger.Info("Found chart: {@result}", result);
            return Ok(result);
        }

        [HttpGet("short-charts")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortCharts([FromQuery] string keyWord = null)
        {
            return Ok(await _chartRepository.GetShortCharts(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Chart), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Create([FromBody] Chart chart)
        {
            _logger.Info("Creating chart: {@chart}", chart);
            chart.Id = DataUtil.GenerateUniqueId();
            await _chartRepository.AddAsync(chart);
            _logger.Info("Created app: {@chart}", chart);

            return Ok(chart);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Update(string id, [FromBody] Chart chart)
        {
            if (ModelState.IsValid)
            {
                chart.Id = id;
                await _chartRepository.UpdateAsync(id, chart);
                _logger.Info("Updated Chart: {@chart}", chart);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _chartRepository.IsExistAsync(a => a.Name == name));
        }

        [HttpGet("extract")]
        [ProducesResponseType(typeof(ExtractionChartFilter), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Extraction([FromBody] ExtractingChartQueryModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _chartService.Extract(model);
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("execution")]
        [ProducesResponseType(typeof(ExecutionChartResponseModel), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Execution([FromBody] ExecutionChartRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var foundChart = await _chartRepository.GetOneAsync(model.ChartId);
                if (foundChart != null)
                {
                    var result = await _chartService.Execute(foundChart, model);
                    return Ok(result);
                }
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _chartRepository.DeleteAsync(id);
            return Ok();
        }

        
        [HttpPost("clone")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone chart with {@model}", model);
            await _chartRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }
    }
}
