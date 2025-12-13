using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Https;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Apps;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Localizations;
using LetPortal.Portal.Services.Apps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/apps")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private readonly IAppRepository _appRepository;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IServiceLogger<AppsController> _logger;

        private readonly ILocalizationRepository _localizationRepository;

        private readonly IAppService _appService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppsController(
            IAppRepository appRepository,
            IPageServiceProvider pageServiceProvider,
            ILocalizationRepository localizationRepository,
            IAppService appService,
            IHttpContextAccessor httpContextAccessor,
            IServiceLogger<AppsController> logger
            )
        {
            _appRepository = appRepository;
            _pageServiceProvider = pageServiceProvider;
            _localizationRepository = localizationRepository;
            _appService = appService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet("{id}/{localeId}")]
        [ProducesResponseType(typeof(App), 200)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> GetOne(string id, string localeId)
        {
            _logger.Info("Getting App with Id = {id}", id);
            var result = await _appRepository.GetOneAsync(id);
            _logger.Info("Found app: {@result}", result);

            var languageKeys = await _localizationRepository.GetAppLangues(id, localeId);
            if(languageKeys != null && languageKeys.Any())
            {
                TranslateApp(result, languageKeys);
            }
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("all-apps")]
        [ProducesResponseType(typeof(List<App>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appRepository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("all/{localeId}")]
        [ProducesResponseType(typeof(List<App>), 200)]
        [Authorize]
        public async Task<IActionResult> GetMany(string localeId, [FromQuery] string ids)
        {
            _logger.Info("Requesting apps with ids = {ids}", ids);
            var idsList = ids.Split(";").ToList();
            var result = await _appRepository.GetAllByIdsAsync(idsList);
            foreach(var app in result)
            {
                var languageKeys = await _localizationRepository.GetAppLangues(app.Id, localeId);
                if(languageKeys != null && languageKeys.Any())
                {
                    TranslateApp(app, languageKeys);
                }
            }
            _logger.Info("Found apps: {@result}", result);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("short-apps")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortApps([FromQuery] string keyWord)
        {
            _logger.Info("Get short apps by keyword = {keyword}", keyWord);
            var result = await _appRepository.GetShortApps(keyWord);
            _logger.Info("Found short apps by keyword {keyword}: {@result}", keyWord, result);
            return Ok(result);
        }

        [HttpGet("{id}/urls")]
        [ProducesResponseType(typeof(IEnumerable<AvailableUrl>), 200)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> GetAvailableUrls(string id)
        {
            // TODO: We will implement a restriction urls per App
            var result = (await _pageServiceProvider.GetAllPages()).Select(a => new AvailableUrl { Name = a.DisplayName, Url = a.UrlPath, PageId = a.Id }).ToList();
            _logger.Info("Found all available urls: {@result}", result);
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(App), 200)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> Create([FromBody] App app)
        {
            _logger.Info("Creating app: {@app}", app);
            app.Id = DataUtil.GenerateUniqueId();
            app.CreatedDate = DateTime.UtcNow;
            app.ModifiedDate = DateTime.UtcNow;
            await _appRepository.AddAsync(app);
            _logger.Info("Created app: {@app}", app);

            return Ok(app);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> Update(string id, [FromBody] App app)
        {
            app.Id = id;
            _logger.Info("Updating app with id = {id} and app info = {@app}", id, app);
            await _appRepository.UpdateAsync(id, app);
            return Ok();
        }

        [HttpPut("{id}/menus")]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> UpdateMenu(string id, [FromBody] List<Menu> menus)
        {
            _logger.Info("Updating menu with app id = {id} and menus = {@menus}", id, menus);
            await _appRepository.UpdateMenuAsync(id, menus);
            return Ok();
        }

        [HttpPut("{id}/menus/assign-role")]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> AsssignRolesToMenu(string id, [FromBody] MenuProfile menuProfile)
        {
            _logger.Info("Updating menu profile with app id = {id} and menu profile = {@menuProfile}", id, menuProfile);
            await _appRepository.UpdateMenuProfileAsync(id, menuProfile);

            return Ok();
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<App>), 200)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> GetAllApps([FromQuery] string localeId)
        {
            var result = await _appRepository.GetAllAsync();
            foreach (var app in result)
            {
                var languageKeys = await _localizationRepository.GetAppLangues(app.Id, localeId);
                if (languageKeys != null && languageKeys.Any())
                {
                    TranslateApp(app, languageKeys);
                }
            }
            _logger.Info("Found apps = {@result}", result);
            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _appRepository.DeleteAsync(id);
            await _localizationRepository.DeleteAll(id);
            return Ok();
        }

        [HttpPost("clone")]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone app with {@model}", model);
            var newId = await _appRepository.CloneAsync(model.CloneId, model.CloneName);
            await _localizationRepository.CloneLocalization(model.CloneId, newId);
            return Ok();
        }

        [HttpPost("{appId}/package")]
        [Authorize(Roles = Roles.BackEndRoles)]
        [ProducesResponseType(typeof(PackageResponseModel), 200)]
        public async Task<IActionResult> Package(string appId, [FromBody] PackageRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(await _appService.Package(model));
        }

        [HttpPost("unpackage")]
        [Authorize(Roles = Roles.BackEndRoles)]
        [ProducesResponseType(typeof(UnpackResponseModel), 200)]
        public async Task<IActionResult> Unpack(IFormFile formFile)
        {
            return Ok(await _appService.Unpack(formFile, _httpContextAccessor.HttpContext.Request.GetJwtToken().GetUserName()));
        }

        [HttpPost("install")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Install([FromBody] InstallRequestModel model)
        {
            await _appService.Install(model.UploadFileId, model.InstallWay);
            return Ok();
        }

        [HttpGet("{appId}/preview")]
        //[Authorize(Roles = Roles.BackEndRoles)]
        [ProducesResponseType(typeof(PreviewPackageModel), 200)]
        public async Task<IActionResult> Preview(string appId)
        {
            return Ok(await _appService.Preview(appId));
        }

        private static void TranslateApp(App app, IEnumerable<LanguageKey> languageKeys)
        {
            var displayName = languageKeys.First(a => a.Key == $"apps.{app.Name}.displayName");
            app.DisplayName = displayName.Value;

            if(app.Menus != null && app.Menus.Count > 0)
            {
                int i = 0;
                foreach(var menu in app.Menus)
                {
                    var menuName = languageKeys.First(a => a.Key == $"apps.{app.Name}.menus[{i.ToString()}].displayName");
                    menu.DisplayName = menuName.Value;

                    if(menu.SubMenus != null && menu.SubMenus.Count > 0)
                    {
                        int j = 0;
                        foreach(var sub in menu.SubMenus)
                        {
                            var subMenuName = languageKeys.First(a => a.Key == $"apps.{app.Name}.menus[{i.ToString()}][{j.ToString()}].displayName");
                            sub.DisplayName = subMenuName.Value;
                            j++;
                        }
                    }
                    i++;
                }
            }
        }
    }
}
