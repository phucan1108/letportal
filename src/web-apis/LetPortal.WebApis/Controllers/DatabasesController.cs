using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Commands;
using LetPortal.Portal.Handlers.Databases.Queries;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/databases")]
    [ApiController]
    public class DatabasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger _logger;

        public DatabasesController(IMediator mediator, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<DatabasesController>();
        }

        // GET: api/Databases
        [HttpGet]
        [ProducesResponseType(typeof(List<DatabaseConnection>), 200)]
        public async Task<IActionResult> Get()
        {
            List<DatabaseConnection> results = await _mediator.Send(new GetAllDatabasesRequest(new GetAllDatabasesQuery()));

            return Ok(results);
        }

        // GET: api/Databases/5
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(typeof(DatabaseConnection), 200)]
        public async Task<IActionResult> Get(string id)
        {
            DatabaseConnection result = await _mediator.Send(new GetOneDatabaseRequest(new GetOneDatabaseQuery { Id = id }));

            return Ok(result);
        }

        // POST: api/Databases
        [HttpPost]
        [ProducesResponseType(typeof(DatabaseConnection), 200)]
        public async Task<IActionResult> Post([FromBody] CreateDatabaseCommand command)
        {
            if(ModelState.IsValid)
            {
                DatabaseConnection result = await _mediator.Send(new CreateDatabaseRequest(command));

                return Ok(result);
            }

            return BadRequest();
        }

        // PUT: api/Databases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateDatabaseCommand command)
        {
            if(ModelState.IsValid)
            {
                await _mediator.Send(new UpdateDatabaseRequest(command));

                return Ok();
            }

            return BadRequest();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(ModelState.IsValid)
            {
                await _mediator.Send(new DeleteDatabaseRequest(new DeleteDatabaseCommand { Id = id }));

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("execution/{databaseId}")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        public async Task<IActionResult> ExecutionDynamic(string databaseId, [FromBody] dynamic content)
        {
            var result = await _mediator.Send(new ExecuteDynamicRequest(new ExecuteDynamicCommand { DatabaseId = databaseId, FormattedCommand = content.ToString() }));

            return Ok(result);
        }

        [HttpPost("extract-raw")]
        [ProducesResponseType(typeof(ExtractingSchemaQueryModel), 200)]
        public async Task<IActionResult> ExtractingQuery([FromBody] ExtractingColumnSchemaQuery extractingColumnSchemaQuery)
        {
            var result = await _mediator.Send(new ExtractingColumnSchemaRequest(extractingColumnSchemaQuery));
            return Ok(result);
        }

        [HttpPost("query-datasource")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        public async Task<IActionResult> ExecuteQueryDatasource([FromBody] ExecuteQueryForDatasourceQuery executeQueryForDatasourceQuery)
        {
            var result = await _mediator.Send(new ExecuteQueryForDatasourceRequest(executeQueryForDatasourceQuery));

            return Ok(result);
        }
    }
}
