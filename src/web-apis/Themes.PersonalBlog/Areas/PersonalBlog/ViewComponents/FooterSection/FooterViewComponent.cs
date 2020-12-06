using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.FooterSection
{
    [ThemePart(BindingType = BindingType.None)]
    public class FooterViewComponent : ViewComponent
    {
        private readonly IThemeProvider _themeProvider;

        private readonly ISiteRequestAccessor _siteRequestAccessor;

        public FooterViewComponent(
            IThemeProvider themeProvider,
            ISiteRequestAccessor siteRequestAccessor)
        {
            _themeProvider = themeProvider;
            _siteRequestAccessor = siteRequestAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = await _themeProvider
                                .LoadAsync(new FooterModel(),
                                    _siteRequestAccessor.Current.Site.Id);
            return View(footerModel);
        }
    }
}
