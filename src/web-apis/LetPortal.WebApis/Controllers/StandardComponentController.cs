using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/standards")]
    [ApiController]
    public class StandardComponentController : ControllerBase
    {
        private readonly IStandardRepository _standardRepository;

        private readonly IServiceLogger<StandardComponentController> _logger;

        public StandardComponentController(
            IStandardRepository standardRepository,
            IServiceLogger<StandardComponentController> logger)
        {
            _standardRepository = standardRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StandardComponent), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetOne(string id)
        {
            var result = await _standardRepository.GetOneAsync(id);
            _logger.Info("Found standard component: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{id}/render")]
        [ProducesResponseType(typeof(StandardComponent), 200)]
        [Authorize]
        public async Task<IActionResult> GetOneForRender(string id)
        {
            var result = await _standardRepository.GetOneForRenderAsync(id);
            _logger.Info("Found standard component: {@result}", result);
            return Ok(result);
        }

        [HttpGet("short-standards")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetSortStandards([FromQuery] string keyWord = null)
        {
            return Ok(await _standardRepository.GetShortStandards(keyWord));
        }

        [HttpGet("short-array-standards")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetSortArrayStandards([FromQuery] string keyWord = null)
        {
            return Ok(await _standardRepository.GetShortArrayStandards(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> CreateOne([FromBody] StandardComponent standardComponent)
        {
            if (ModelState.IsValid)
            {
                standardComponent.Id = DataUtil.GenerateUniqueId();
                await _standardRepository.AddAsync(standardComponent);
                _logger.Info("Created standard component: {@standardComponent}", standardComponent);
                return Ok(standardComponent.Id);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> UpdateOne(string id, [FromBody] StandardComponent standardComponent)
        {
            if (ModelState.IsValid)
            {
                standardComponent.Id = id;
                await _standardRepository.UpdateAsync(id, standardComponent);
                _logger.Info("Updated standard component: {@standardComponent}", standardComponent);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("bulk")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> CreateBulk([FromBody] List<StandardComponent> standardComponents)
        {
            standardComponents.ForEach(a =>
            {
                a.Id = DataUtil.GenerateUniqueId();
            });
            _logger.Info("Created standard components: {@standardComponents}", standardComponents);
            await _standardRepository.AddBulkAsync(standardComponents);
            return Ok();
        }

        [HttpGet("bulk")]
        [ProducesResponseType(typeof(List<StandardComponent>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetManys([FromQuery] string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var result = await _standardRepository
                    .GetAllByIdsAsync(
                        ids.Split(";").ToList(),
                        expression: a => a.AllowArrayData == false,
                        isRequiredDiscriminator: true);
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
            else
            {
                var result = await _standardRepository
                                .GetAllAsync(
                                    expression: a => a.AllowArrayData == false,
                                    isRequiredDiscriminator: true);
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
        }

        [HttpGet("bulk-array")]
        [ProducesResponseType(typeof(List<StandardComponent>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetArrayManys([FromQuery] string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var result = await _standardRepository
                                .GetAllByIdsAsync(
                                    ids.Split(";").ToList(),
                                    expression: a => a.AllowArrayData == true,
                                    isRequiredDiscriminator: true);
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
            else
            {
                var result = await _standardRepository.GetAllAsync(a => a.AllowArrayData == true, isRequiredDiscriminator: true);
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _standardRepository.IsExistAsync(a => a.Name == name));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> Delete(string id)
        {
            await _standardRepository.DeleteAsync(id);
            return Ok();
        }


        [HttpPost("clone")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone Standard with {@model}", model);
            await _standardRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }
    }
}
