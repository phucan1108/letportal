﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Models.Apps;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Apps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/apps")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private readonly IAppRepository _appRepository;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IServiceLogger<AppsController> _logger;

        public AppsController(
            IAppRepository appRepository,
            IPageServiceProvider pageServiceProvider,
            IServiceLogger<AppsController> logger
            )
        {
            _appRepository = appRepository;
            _pageServiceProvider = pageServiceProvider;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App), 200)]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> GetOne(string id)
        {
            _logger.Info("Getting App with Id = {id}", id);
            var result = await _appRepository.GetOneAsync(id);
            _logger.Info("Found app: {@result}", result);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<App>), 200)]
        [Authorize]
        public async Task<IActionResult> GetMany([FromQuery] string ids)
        {
            _logger.Info("Requesting apps with ids = {ids}", ids);
            var idsList = ids.Split(";").ToList();
            var result = await _appRepository.GetAllByIdsAsync(idsList);
            _logger.Info("Found apps: {@result}", result);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("short-apps")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = RolesConstants.BACK_END_ROLES)]
        public async Task<IActionResult> GetShortApps([FromQuery] string keyWord)
        {
            _logger.Info("Get short apps by keyword = {keyword}", keyWord);
            var result = await _appRepository.GetShortApps(keyWord);
            _logger.Info("Found short apps by keyword {keyword}: {@result}", keyWord, result);
            return Ok(result);
        }

        [HttpGet("{id}/urls")]
        [ProducesResponseType(typeof(IEnumerable<AvailableUrl>), 200)]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> GetAvailableUrls(string id)
        {
            // TODO: We will implement a restriction urls per App
            var result = (await _pageServiceProvider.GetAllPages()).Select(a => new AvailableUrl { Name = a.DisplayName, Url = a.UrlPath, PageId = a.Id }).ToList();
            _logger.Info("Found all available urls: {@result}", result);
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(App), 200)]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
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
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> Update(string id, [FromBody] App app)
        {
            app.Id = id;
            _logger.Info("Updating app with id = {id} and app info = {@app}", id, app);
            await _appRepository.UpdateAsync(id, app);
            return Ok();
        }

        [HttpPut("{id}/menus")]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> UpdateMenu(string id, [FromBody] List<Menu> menus)
        {
            _logger.Info("Updating menu with app id = {id} and menus = {@menus}", id, menus);
            await _appRepository.UpdateMenuAsync(id, menus);
            return Ok();
        }

        [HttpPut("{id}/menus/assign-role")]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> AsssignRolesToMenu(string id, [FromBody] MenuProfile menuProfile)
        {
            _logger.Info("Updating menu profile with app id = {id} and menu profile = {@menuProfile}", id, menuProfile);
            await _appRepository.UpdateMenuProfileAsync(id, menuProfile);

            return Ok();
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<App>), 200)]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> GetAllApps()
        {
            var result = await _appRepository.GetAllAsync();
            _logger.Info("Found apps = {@result}", result);
            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> Delete(string id)
        {
            await _appRepository.DeleteAsync(id);
            return Ok();
        }

        [HttpPost("clone")]
        [Authorize(Roles = RolesConstants.ALL_ADMIN_ROLES)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone app with {@model}", model);
            await _appRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }
    }
}
