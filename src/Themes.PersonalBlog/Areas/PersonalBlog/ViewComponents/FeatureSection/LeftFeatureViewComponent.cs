using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Services.Pages;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.FeatureSection
{
    [ThemePart(BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Object, Type = typeof(LeftFeatureModel))]
    public class LeftFeatureViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IPageService _pageService;

        public LeftFeatureViewComponent(
            ISiteRequestAccessor siteRequest,
            IPageService pageService)
        {
            _siteRequest = siteRequest;
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string templateKey)
        {
            var leftFeatureModel = await _pageService.LoadModel<LeftFeatureModel>(templateKey, _siteRequest);
            return View(leftFeatureModel);
        }
    }
}
