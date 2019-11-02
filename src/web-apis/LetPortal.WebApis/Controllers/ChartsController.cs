using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Charts;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Services.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LetPortal.PortalApis.Controllers
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Chart), 200)]
        public async Task<IActionResult> GetOne(string id)
        {
            _logger.Info("Getting Chart with Id = {id}", id);
            var result = await _chartRepository.GetOneAsync(id);
            _logger.Info("Found chart: {@result}", result);
            if(result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Chart), 200)]
        public async Task<IActionResult> Create([FromBody] Chart chart)
        {
            _logger.Info("Creating chart: {@chart}", chart);
            chart.Id = DataUtil.GenerateUniqueId();
            await _chartRepository.AddAsync(chart);
            _logger.Info("Created app: {@chart}", chart);

            return Ok(chart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Chart chart)
        {
            if(ModelState.IsValid)
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
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _chartRepository.IsExistAsync(a => a.Name == name));
        }

        [HttpGet("extract")]
        [ProducesResponseType(typeof(ExtractionChartFilter), 200)]
        public async Task<IActionResult> Extraction([FromBody] ExtractingChartQueryModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _chartService.Extract(model);
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("execution")]
        [ProducesResponseType(typeof(ExecutionChartResponseModel), 200)]
        public async Task<IActionResult> Execution([FromBody] ExecutionChartRequestModel model)
        {
            if(ModelState.IsValid)
            {
                var foundChart = await _chartRepository.GetOneAsync(model.ChartId);
                if(foundChart != null)
                {
                    var result = await _chartService.Execute(foundChart, model);
                    return Ok(result);
                }
            }
            return BadRequest();
        }

    }
}
