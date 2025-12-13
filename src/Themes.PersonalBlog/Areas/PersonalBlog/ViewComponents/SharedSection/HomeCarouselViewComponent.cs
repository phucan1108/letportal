using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Services.Pages;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.SharedSection
{
    [ThemePart(BindingType = BindingType.Array, Type = typeof(HomeCarouselModel))]
    public class HomeCarouselViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IPageService _pageService;

        public HomeCarouselViewComponent(
            ISiteRequestAccessor siteRequest,
            IPageService pageService)
        {
            _siteRequest = siteRequest;
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string templateKey)
        {
            var homeCarousels = await _pageService.LoadModel<HomeCarouselModel>(templateKey, _siteRequest);
            return View(homeCarousels);
        }
    }
}
