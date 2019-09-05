using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Commands;
using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/entity-schemas")]
    [ApiController]
    public class EntitySchemasController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger _logger;

        public EntitySchemasController(IMediator mediator, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<EntitySchemasController>();
        }

        [HttpGet("{entityName}/{databaseId}")]
        [ProducesResponseType(typeof(EntitySchema), 200)]
        public async Task<IActionResult> GetOne(string databaseId, string entityName)
        {
            var result = await _mediator.Send(new GetOneEntitySchemaByNameRequest(new GetOneEntitySchemaByNameQuery { DatabaseId = databaseId, EntityName = entityName }));
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        [HttpGet("fetch/{id}")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        public async Task<IActionResult> FetchAllFromDatabase(string id)
        {
            List<EntitySchema> result = await _mediator.Send(new FetchAllEntitiesFromDatabaseRequest(new FetchAllEntitiesFromDatabaseQuery { DatabaseId = id }));

            return Ok(result);
        }

        [HttpGet("database/{id}")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        public async Task<IActionResult> GetAllFromOneDatabase(string id)
        {
            List<EntitySchema> result = await _mediator.Send(new GetAllEntitySchemasOfOneDatabaseRequest(new GetAllEntitySchemasOfOneDatabaseQuery { DatabaseId = id }));

            return Ok(result);
        }

        [HttpPost("flush")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        public async Task<IActionResult> FlushOneDatabase(FlushEntitySchemasInOneDatabaseCommand flushEntitySchemasInOneDatabaseCommand)
        {
            List<EntitySchema> result = await _mediator.Send(new FlushEntitySchemasInOneDatabaseRequest(flushEntitySchemasInOneDatabaseCommand));

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EntitySchema), 200)]
        public async Task<IActionResult> Post([FromBody] CreateEntitySchemaCommand createEntitySchemaCommand)
        {
            if (ModelState.IsValid)
            {
                EntitySchema result = await _mediator.Send(new CreateEntitySchemaRequest(createEntitySchemaCommand));

                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateEntitySchemaCommand updateEntitySchemaCommand)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateEntitySchemaRequest(updateEntitySchemaCommand));

                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteEntitySchemaRequest(new DeleteEntitySchemaCommand { EntitySchemaId = id }));

            return Ok();
        }
    }
}