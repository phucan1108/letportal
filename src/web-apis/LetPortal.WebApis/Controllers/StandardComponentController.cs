using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Components;
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
        public async Task<IActionResult> GetOne(string id)
        {
            var result = await _standardRepository.GetOneAsync(id);
            _logger.Info("Found standard component: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{id}/render")]
        [ProducesResponseType(typeof(StandardComponent), 200)]
        public async Task<IActionResult> GetOneForRender(string id)
        {
            var result = await _standardRepository.GetOneForRenderAsync(id);
            _logger.Info("Found standard component: {@result}", result);
            return Ok(result);
        }

        [HttpGet("short-standards")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        public async Task<IActionResult> GetSortStandards([FromQuery] string keyWord = null)
        {
            return Ok(await _standardRepository.GetShortStandards(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
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

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteBulk([FromQuery] string ids)
        {
            _logger.Info("Deleted bulk standard components: {ids}", ids);
            await _standardRepository.DeleteBulkAsync(ids.Split(";"));
            return Ok();
        }

        [HttpGet("bulk")]
        [ProducesResponseType(typeof(List<StandardComponent>), 200)]
        public async Task<IActionResult> GetManys([FromQuery] string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var result = await _standardRepository.GetAllByIdsAsync(ids.Split(";").ToList());
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
            else
            {
                var result = await _standardRepository.GetAllAsync(isRequiredDiscriminator: true);
                _logger.Info("Get bulk standard components: {@result}", result);
                return Ok(result);
            }
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _standardRepository.IsExistAsync(a => a.Name == name));
        }
    }
}
