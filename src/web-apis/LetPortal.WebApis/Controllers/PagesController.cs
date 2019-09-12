using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageRepository _pageRepository;

        private readonly IServiceLogger<PagesController> _logger;

        public PagesController(
            IPageRepository pageRepository,
            IServiceLogger<PagesController> logger)
        {
            _pageRepository = pageRepository;
            _logger = logger;
        }

        [HttpGet("shorts")]
        [ProducesResponseType(typeof(List<ShortPageModel>), 200)]
        public async Task<IActionResult> GetAllShortPages()
        {
            var result = await _pageRepository.GetAllShortPages();
            _logger.Info("All short pages: {@result}", result);
            return Ok(result);
        }

        [HttpGet("all-claims")]
        [ProducesResponseType(typeof(List<ShortPortalClaimModel>), 200)]
        public async Task<IActionResult> GetAllPortalClaims()
        {
            var result = await _pageRepository.GetShortPortalClaimModels();
            _logger.Info("All portal claims: {@result}", result);
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOneById(string id)
        {
            var result = await _pageRepository.GetOneAsync(id);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOne(string name)
        {
            var result = await _pageRepository.GetOneByName(name);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Create([FromBody] Page page)
        {
            if(ModelState.IsValid)
            {
                page.Id = DataUtil.GenerateUniqueId();
                await _pageRepository.AddAsync(page);
                _logger.Info("Created page: {@page}", page);
                return Ok(page.Id);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Page page)
        {
            if(ModelState.IsValid)
            {
                page.Id = id;
                await _pageRepository.UpdateAsync(id, page);
                _logger.Info("Updated page: {@page}", page);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _pageRepository.DeleteAsync(id);
            _logger.Info("Deleted page id: {id}", id);
            return Ok();
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _pageRepository.IsExistAsync(name));
        }
    }
}
