using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Services.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/dynamiclists")]
    [ApiController]
    public class DynamicListController : ControllerBase
    {
        private readonly IDynamicListRepository _dynamicListRepository;

        private readonly IDynamicListService _dynamicService;

        private readonly IServiceLogger<DynamicListController> _logger;

        public DynamicListController(
            IDynamicListRepository dynamicListRepository,
            IDynamicListService dynamicListService,
            IServiceLogger<DynamicListController> logger)
        {
            _dynamicListRepository = dynamicListRepository;
            _dynamicService = dynamicListService;
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<DynamicList>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dynamicListRepository.GetAllAsync(isRequiredDiscriminator: true);
            _logger.Info("Found dynamic lists {@result}", result);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DynamicList), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetOne(string id)
        {
            var result = await _dynamicListRepository.GetOneAsync(id);
            _logger.Info("Found dynamic list: {@result}", result);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}/render")]
        [ProducesResponseType(typeof(DynamicList), 200)]
        [Authorize]
        public async Task<IActionResult> GetOneForRender(string id)
        {
            var result = await _dynamicListRepository.GetOneAsync(id);
            _logger.Info("Found dynamic list: {@result}", result);

            // Remove some security aspects
            if(result.ListDatasource.DatabaseConnectionOptions != null)
            {
                result.ListDatasource.DatabaseConnectionOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(result.ListDatasource.DatabaseConnectionOptions.Query, true));
            }

            if(result.CommandsList?.CommandButtonsInList != null)
            {
                foreach(var command in result.CommandsList.CommandButtonsInList.Where(a => a.ActionCommandOptions.ActionType == Portal.Entities.Shared.ActionType.ExecuteDatabase))
                {
                    foreach(var step in command.ActionCommandOptions.DbExecutionChains.Steps)
                    {
                        step.ExecuteCommand = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(step.ExecuteCommand, true));
                    }
                }
            }

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("short-dynamiclists")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortDynamicLists([FromQuery] string keyWord = null)
        {
            return Ok(await _dynamicListRepository.GetShortDynamicLists(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(DynamicList), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Create([FromBody] DynamicList dynamicList)
        {
            if (ModelState.IsValid)
            {
                dynamicList.Id = DataUtil.GenerateUniqueId();
                await _dynamicListRepository.AddAsync(dynamicList);
                _logger.Info("Created dynamic list: {@dynamicList}", dynamicList);
                return Ok(dynamicList);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Update(string id, [FromBody] DynamicList dynamicList)
        {
            if (ModelState.IsValid)
            {
                dynamicList.Id = id;
                await _dynamicListRepository.UpdateAsync(id, dynamicList);
                _logger.Info("Updated dynamic list: {@dynamicList}", dynamicList);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _dynamicListRepository.DeleteAsync(id);
            _logger.Info("Deleted dynamic list {id}", id);
            return Ok();
        }

        [HttpPost("{id}/fetch-data")]
        [ProducesResponseType(typeof(DynamicListResponseDataModel), 200)]
        [Authorize]
        public async Task<IActionResult> ExecuteQuery(string id, [FromBody] DynamicListFetchDataModel fetchDataModel)
        {
            _logger.Info("Execute query in dynamic list id {id} with fetch data {@fetchDataModel}", id, fetchDataModel);
            var dynamicList = await _dynamicListRepository.GetOneAsync(id);

            if (dynamicList == null)
            {
                return NotFound();
            }

            var result = await _dynamicService.FetchData(dynamicList, fetchDataModel);
            _logger.Info("Result of query: {@result}", result);
            return Ok(result);
        }

        [HttpPost("extract-query")]
        [ProducesResponseType(typeof(PopulateQueryModel), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> ExtractingQuery([FromBody] ExtractingQueryModel extractingQuery)
        {
            _logger.Info("Extracing query model {@extractingQuery}", extractingQuery);
            var result = await _dynamicService.ExtractingQuery(extractingQuery);
            _logger.Info("Extracting result {@result}", result);
            return Ok(result);
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _dynamicListRepository.IsExistAsync(a => a.Name == name));
        }

        
        [HttpPost("clone")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone dynamic list with {@model}", model);
            await _dynamicListRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }
    }
}
