using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Models;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Services.Datasources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/datasources")]
    [ApiController]
    public class DatasourceController : ControllerBase
    {
        private readonly IDatasourceRepository _datasourceRepository;

        private readonly IDatasourceService _datasourceService;

        private readonly IServiceLogger<DatasourceController> _logger;
        // Notes: It is following the In-Memory Cache of Microsoft https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
        // Later, we need to change its by distributed cache.
        private readonly IMemoryCache _memoryCache;

        public DatasourceController(
            IDatasourceRepository datasourceRepository,
            IDatasourceService datasourceService,
            IServiceLogger<DatasourceController> logger,
            IMemoryCache memoryCache)
        {
            _datasourceRepository = datasourceRepository;
            _datasourceService = datasourceService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<Datasource>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _datasourceRepository.GetAllAsync();

            _logger.Info("Found all datasources: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Datasource), 200)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _datasourceRepository.GetOneAsync(id);
            _logger.Info("Getting datasource id {id}, found datasource: {@result}", id, result);
            return Ok(result);
        }

        [HttpGet("fetch/{id}")]
        [ProducesResponseType(typeof(List<DatasourceModel>), 200)]
        public async Task<IActionResult> FetchDatasource(string id, [FromQuery] string keyWord)
        {
            if(!_memoryCache.TryGetValue(id, out List<DatasourceModel> result))
            {
                // Important note: we should fetch all and cache if needed for best throughput
                var datasource = await _datasourceRepository.GetOneAsync(id);
                if(datasource != null)
                {
                    var executedDataSource = await _datasourceService.GetDatasourceService(datasource);

                    if(executedDataSource.CanCache)
                    {
                        _memoryCache.Set(id, result);
                    }

                    result = executedDataSource.DatasourceModels;
                }
                else
                {
                    return BadRequest();
                }
            }
            if(!string.IsNullOrEmpty(keyWord))
            {
                result = result.Where(a => a.Name.Contains(keyWord)).ToList();
            }

            _logger.Info("Found datasource: {@result}", result);

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Datasource), 200)]
        public async Task<IActionResult> Create([FromBody] Datasource datasource)
        {
            if(ModelState.IsValid)
            {
                datasource.Id = DataUtil.GenerateUniqueId();
                await _datasourceRepository.AddAsync(datasource);
                _logger.Info("Created datasource: {@datasource}", datasource);
                return Ok(datasource);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Datasource datasource)
        {
            if(ModelState.IsValid)
            {
                datasource.Id = id;
                await _datasourceRepository.UpdateAsync(id, datasource);
                _logger.Info("Updated datasource with {id}: {@datasource}", id, datasource);
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _datasourceRepository.DeleteAsync(id);
            _logger.Info("Deleted datasource with {id}", id);
            return Ok();
        }
    }
}