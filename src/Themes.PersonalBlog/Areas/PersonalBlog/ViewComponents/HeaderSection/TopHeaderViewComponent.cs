using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.HeaderSection
{
    public class TopHeaderViewComponent : ViewComponent
    {
        private readonly IThemeProvider _themeProvider;

        private readonly ISiteRequestAccessor _siteRequestAccessor;

        public TopHeaderModel TopHeader { get; set; }

        public TopHeaderViewComponent(
            IThemeProvider themeProvider,
            ISiteRequestAccessor siteRequestAccessor)
        {
            _themeProvider = themeProvider;
            _siteRequestAccessor = siteRequestAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            TopHeader = await _themeProvider
                                .LoadAsync(new TopHeaderModel(), 
                                    _siteRequestAccessor.Current.Site.Id);
            return View(TopHeader);
        }
    }
}
