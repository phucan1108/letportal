using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Providers.Localizations;
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

        private readonly ILocalizationProvider _localizationProvider;

        private readonly IServiceLogger<LocalizationController> _logger;

        public LocalizationController(
            ILocalizationRepository localizationRepository,
            ILocalizationProvider localizationProvider,
            IServiceLogger<LocalizationController> logger)
        {
            _localizationRepository = localizationRepository;
            _localizationProvider = localizationProvider;
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

        [HttpGet("locale/{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize]
        public async Task<IActionResult> GetOne(string localeId)
        {
            var result = await _localizationRepository.GetByLocaleId(localeId);
            _logger.Info("Found localization: {@result}", result);
            return Ok(result);
        }

        [HttpGet("exist/{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> CheckExist(string localeId)
        {
            return Ok(await _localizationRepository.CheckLocaleExisted(localeId));
        }

        [HttpGet("collectAll")]
        [ProducesResponseType(typeof(IEnumerable<LanguageKey>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> CollectAll()
        {
            return Ok(await _localizationProvider.CollectAlls());
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
