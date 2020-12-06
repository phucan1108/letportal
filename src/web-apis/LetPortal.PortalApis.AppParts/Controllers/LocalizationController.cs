using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Providers.Localizations;
using LetPortal.Portal.Repositories.Localizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
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

        [HttpGet("{appId}/{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize]
        public async Task<IActionResult> GetOne(string appId ,string localeId)
        {
            var result = await _localizationRepository.GetByLocaleId(localeId, appId);
            _logger.Info("Found localization: {@result}", result);
            return Ok(result);
        }

        [HttpGet("exist/{appId}/{localeId}")]
        [ProducesResponseType(typeof(Localization), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> CheckExist(string appId, string localeId)
        {
            return Ok(await _localizationRepository.CheckLocaleExisted(localeId, appId));
        }

        [HttpGet("collectAll/{appId}")]
        [ProducesResponseType(typeof(IEnumerable<LanguageKey>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> CollectAll(string appId)
        {
            return Ok(await _localizationProvider.CollectAlls(appId));
        }

        [HttpPost("")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Create([FromBody] Localization localization)
        {
            await _localizationRepository.AddAsync(localization);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _localizationRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
