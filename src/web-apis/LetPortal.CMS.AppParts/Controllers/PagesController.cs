using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Repositories.Pages;
using LetPortal.CMS.Core.Repositories.Sites;
using LetPortal.CMS.Core.Repositories.Themes;
using LetPortal.CMS.Core.Security;
using LetPortal.CMS.Core.Services.Pages;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.CMS.AppParts.Controllers
{
    [Route("api/cms-pages")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageRepository _pageRepository;

        private readonly IPageVersionRepository _pageVersionRepository;

        private readonly IPageTemplateRepository _pageTemplateRepository;

        private readonly IThemeRepository _themeRepository;

        private readonly ISiteRepository _siteRepository;

        private readonly IPageService _pageService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagesController(
            IPageRepository pageRepository,
            IPageVersionRepository pageVersionRepository,
            IHttpContextAccessor httpContextAccessor,
            IThemeRepository themeRepository,
            IPageTemplateRepository pageTemplateRepository,
            IPageService pageService)
        {
            _pageRepository = pageRepository;
            _pageVersionRepository = pageVersionRepository;
            _httpContextAccessor = httpContextAccessor;
            _themeRepository = themeRepository;
            _pageTemplateRepository = pageTemplateRepository;
            _pageService = pageService;
        }

        [HttpPost("")]
        [Authorize(Roles = Roles.SiteAdmin)]
        public async Task<IActionResult> Create([FromBody] Page page)
        {
            page.Id = DataUtil.GenerateUniqueId();
            var chosenSite = await _siteRepository.GetOneAsync(page.SiteId);
            var chosenTheme = await _themeRepository.GetOneAsync(chosenSite.ThemeId);
            var chosenPageTemplate = await _pageTemplateRepository.GetOneAsync(page.PageTemplateId);
            //var newVersion = new PageVersion
            //{
            //    Id = DataUtil.GenerateUniqueId(),
            //    CreatedDate = DateTime.UtcNow,
            //    Creator = _httpContextAccessor.HttpContext.User.Identity.Name,
            //    Name = "1",
            //    PageId = page.Id,
            //    Manifests = await _pageService.InitManifests(chosenPageTemplate, chosenTheme)
            //};

            //page.ChosenPageVersionId = newVersion.Id;
            await _pageRepository.AddAsync(page);
            //await _pageVersionRepository.AddAsync(newVersion);
            return Ok();
        }
    }
}
