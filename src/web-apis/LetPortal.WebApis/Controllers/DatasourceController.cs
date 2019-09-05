using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Commands;
using LetPortal.Portal.Handlers.Datasources.Queries;
using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Models;
using LetPortal.Services.Databases.Handlers.Datasources.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/datasources")]
    [ApiController]
    public class DatasourceController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger _logger;
        // Notes: It is following the In-Memory Cache of Microsoft https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
        // Later, we need to change its by distributed cache.
        private readonly IMemoryCache _memoryCache;

        public DatasourceController(IMediator mediator, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<DatasourceController>();
            _memoryCache = memoryCache;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<Datasource>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllDatasourceRequest(new GetAllDatasourceQuery())));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Datasource), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _mediator.Send(new GetOneDatasourceRequest(new GetOneDatasourceQuery { DatasourceId = id })));
        }

        [HttpGet("fetch/{id}")]
        [ProducesResponseType(typeof(List<DatasourceModel>), 200)]
        public async Task<IActionResult> FetchDatasource(string id, [FromQuery] string keyWord)
        {
            if (!_memoryCache.TryGetValue(id, out List<DatasourceModel> result))
            {
                // Important note: we should fetch all and cache if needed for best throughput
                var executedDataSource = await _mediator.Send(new ExecuteDatasourceRequest(new ExecuteDatasourceQuery { KeyWord = keyWord, DatasourceId = id }));

                if (executedDataSource.CanCache)
                {
                    _memoryCache.Set(id, result);
                }

                result = executedDataSource.DatasourceModels;
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                result = result.Where(a => a.Name.Contains(keyWord)).ToList();
            }

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Datasource), 200)]
        public async Task<IActionResult> Create([FromBody] CreateDatasourceCommand createDatasourceCommand)
        {
            return Ok(await _mediator.Send(new CreateDatasourceRequest(createDatasourceCommand)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateDatasourceCommand updateDatasourceCommand)
        {
            updateDatasourceCommand.DatasourceId = id;
            await _mediator.Send(new UpdateDatasourceRequest(updateDatasourceCommand));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteDatasourceRequest(new DeleteDatasourceCommand { DatasourceId = id }));

            return Ok();
        }
    }
}