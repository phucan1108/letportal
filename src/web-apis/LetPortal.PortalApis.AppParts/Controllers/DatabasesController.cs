using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Services.Databases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/databases")]
    [ApiController]
    public class DatabasesController : ControllerBase
    {
        private readonly IDatabaseRepository _databaseRepository;

        private readonly IDatabaseService _databaseService;

        private readonly IServiceLogger<DatabasesController> _logger;

        public DatabasesController(
            IDatabaseRepository databaseRepository,
            IDatabaseService databaseService,
            IServiceLogger<DatabasesController> logger
            )
        {
            _databaseRepository = databaseRepository;
            _databaseService = databaseService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DatabaseConnection>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Get()
        {
            var result = await _databaseRepository.GetAllAsync();
            _logger.Info("Found database connections: {@result}", result);
            if (!result.Any())
            {
                return NotFound();
            }

            // Empty all connection strings for security risks
            foreach (var databaseConnection in result)
            {
                databaseConnection.ConnectionString = string.Empty;
                databaseConnection.DataSource = string.Empty;
            }
            return Ok(result);
        }

        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(typeof(DatabaseConnection), 200)]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            _logger.Info("Requesting to get database id = {id}", id);
            var result = await _databaseRepository.GetOneAsync(id);
            _logger.Info("Found database = {@result}", result);
            if (result == null)
            {
                return NotFound();
            }

            result.ConnectionString = string.Empty;
            result.DataSource = string.Empty;
            return Ok(result);
        }

        [HttpGet("short-databases")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortDatabases([FromQuery] string keyWord = null)
        {
            return Ok(await _databaseRepository.GetShortDatatabases(keyWord));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DatabaseConnection), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Post([FromBody] DatabaseConnection databaseConnection)
        {
            if (ModelState.IsValid)
            {
                databaseConnection.Id = DataUtil.GenerateUniqueId();
                _logger.Info("Creating database: {@databaseConnection}", databaseConnection);
                await _databaseRepository.AddAsync(databaseConnection);

                return Ok(databaseConnection);
            }
            _logger.Info("Creating database isn't correct format. Errors: {@error}", ModelState.Values.SelectMany(a => a.Errors));
            return BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Put(string id, [FromBody] DatabaseConnection databaseConnection)
        {
            if (ModelState.IsValid)
            {
                databaseConnection.Id = id;
                _logger.Info("Updating database: {@databaseConnection}", databaseConnection);
                await _databaseRepository.UpdateAsync(id, databaseConnection);

                return Ok();
            }
            _logger.Info("Updating database isn't correct format. Errors: {@error}", ModelState.Values.SelectMany(a => a.Errors));
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.Info("Deleting database id = {id}", id);
            await _databaseRepository.DeleteAsync(id);

            return Ok();
        }

        [HttpPost("{databaseId}/execution")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> ExecutionDynamic(string databaseId, [FromBody] dynamic content)
        {
            _logger.Info("Execution dynamic in database id = {databaseId} with content = {@content}", databaseId, content);
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);
            if (databaseConnection == null)
            {
                return BadRequest();
            }

            string formattedContent;
            if (databaseConnection.GetConnectionType() != Core.Persistences.ConnectionType.MongoDB)
            {
                formattedContent = content;
            }
            else
            {
                if (content.GetType() == typeof(string))
                {
                    formattedContent = content;
                }
                else
                {
                    formattedContent = ConvertUtil.SerializeObject(content);
                }

            }

            var result = await _databaseService.ExecuteDynamic(databaseConnection, formattedContent, new List<ExecuteParamModel>());
            _logger.Info("Result of execution dynamic: {@result}", result);
            return Ok(result);
        }

        [HttpPost("{databaseId}/extract-raw")]
        [ProducesResponseType(typeof(ExtractingSchemaQueryModel), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> ExtractingQuery(string databaseId, [FromBody] ExtractionDatabaseRequestModel model)
        {
            _logger.Info("Extracting query dynamic in database id = {databaseId} with model = {@model}", databaseId, model);
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);
            if (databaseConnection == null)
            {
                return BadRequest();
            }

            string formattedContent;
            if (databaseConnection.GetConnectionType() != Core.Persistences.ConnectionType.MongoDB)
            {
                formattedContent = model.Content;
            }
            else
            {
                if(model.Content.GetType() == typeof(string))
                {
                    formattedContent = model.Content;
                }
                else
                {
                    formattedContent = ConvertUtil.SerializeObject(model.Content);
                }
                
            }

            var result = await _databaseService.ExtractColumnSchema(databaseConnection, formattedContent, model.Parameters);
            _logger.Info("Result of extracting dynamic: {@result}", result);
            return Ok(result);
        }

        [HttpPost("{databaseId}/query-datasource")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> ExecuteQueryDatasource(string databaseId, [FromBody] dynamic content)
        {
            _logger.Info("Execute query for datasource in database id = {databaseId} with content = {@content}", databaseId, content);
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);
            if (databaseConnection == null)
            {
                return BadRequest();
            }

            var result = await _databaseService.ExecuteDynamic(databaseConnection, content, new List<ExecuteParamModel>());
            _logger.Info("Result of dynamic datasource: {@result}", result);
            return Ok(result);
        }

        [HttpPost("clone")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone database with {@model}", model);
            await _databaseRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }
    }
}
