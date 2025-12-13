using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/composite-controls")]
    [ApiController]
    public class CompositeControlsController : ControllerBase
    {
        private readonly IServiceLogger<CompositeControlsController> _logger;

        private readonly ICompositeControlRepository _controlRepository;

        public CompositeControlsController(
            IServiceLogger<CompositeControlsController> logger,
            ICompositeControlRepository controlRepository)
        {
            _logger = logger;
            _controlRepository = controlRepository;
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CompositeControl), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _controlRepository.GetOneAsync(id)); 
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(List<CompositeControl>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Ok(await _controlRepository.GetAllAsync());
            }
            else
            {
                var idsList = ids.Split(";").ToList();
                return Ok(await _controlRepository.GetAllByIdsAsync(idsList));
            }
        }

        [HttpGet("short-controls")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortControls([FromQuery] string keyWord = null)
        {
            return Ok(await _controlRepository.GetShortEntities(keyWord));
        }

        [HttpPost("")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Create([FromBody] CompositeControl model)
        {
            model.Id = DataUtil.GenerateUniqueId();
            await _controlRepository.AddAsync(model);

            return Ok();
        }
             

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Update(string id, [FromBody] CompositeControl model)
        {
            if(model.Id == id)
            {
                await _controlRepository.UpdateAsync(id, model);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _controlRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
