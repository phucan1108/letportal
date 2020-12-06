using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Models.EntitySchemas;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Services.EntitySchemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/entity-schemas")]
    [ApiController]
    public class EntitySchemasController : ControllerBase
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        private readonly IEntitySchemaService _entitySchemaService;

        private readonly IServiceLogger<EntitySchemasController> _logger;

        public EntitySchemasController(
            IEntitySchemaRepository entitySchemaRepository,
            IEntitySchemaService entitySchemaService,
            IServiceLogger<EntitySchemasController> logger)
        {
            _entitySchemaRepository = entitySchemaRepository;
            _entitySchemaService = entitySchemaService;
            _logger = logger;
        }

        [HttpGet("{entityName}/{databaseId}")]
        [ProducesResponseType(typeof(EntitySchema), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetOne(string databaseId, string entityName)
        {
            _logger.Info("Get one entity schema in database id = {databaseId}, entity name = {entityName}", databaseId, entityName);
            var result = await _entitySchemaRepository.GetOneEntitySchemaAsync(databaseId, entityName);
            _logger.Info("Found entity schemas: {@result}", result);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("fetch/{id}")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> FetchAllFromDatabase(string id)
        {
            var result = await _entitySchemaService.FetchAllEntitiesFromDatabase(id);

            return Ok(result);
        }

        [HttpGet("database/{id}")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetAllFromOneDatabase(string id)
        {
            var result = await _entitySchemaRepository.GetAllAsync(a => a.DatabaseId == id);

            return Ok(result);
        }

        [HttpPost("flush")]
        [ProducesResponseType(typeof(List<EntitySchema>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> FlushOneDatabase(FlushDatabaseModel flushDatabaseModel)
        {
            var result = await _entitySchemaService.FetchAllEntitiesFromDatabase(flushDatabaseModel.DatabaseId);

            await _entitySchemaRepository.UpsertEntitySchemasAsync(result, flushDatabaseModel.DatabaseId, flushDatabaseModel.KeptSameName);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _entitySchemaRepository.DeleteAsync(id);
            _logger.Info("Deleted entity schema id = {id}", id);
            return Ok();
        }
    }
}
