using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Repositories.Localizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LetPortal.PortalApis.Controllers
{
    [Route("api/localizations")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly ILocalizationRepository _localizationRepository;

        private readonly IServiceLogger<LocalizationController> _logger;

        public LocalizationController(
            ILocalizationRepository localizationRepository,
            IServiceLogger<LocalizationController> logger)
        {
            _localizationRepository = localizationRepository;
            _logger = logger;
        }

        [HttpGet("{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize]
        public async Task<IActionResult> GetOneBuilder(string localeId)
        {
            var result = await _localizationRepository.GetOneAsync(localeId);
            _logger.Info("Found localization: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{pageId}/{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize]
        public async Task<IActionResult> GetOne(string pageId, string localeId)
        {
            var result = await _localizationRepository.GetByPageIdAndLocaleId(pageId, localeId);
            _logger.Info("Found localization: {@result}", result);
            return Ok(result);
        }

        [HttpPost("")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> Create([FromBody] Localization localization)
        {
            await _localizationRepository.AddAsync(localization);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> Delete(string id)
        {
            await _localizationRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
