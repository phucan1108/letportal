using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContentManagement.Themes.TheFace.Areas.TheFace.Pages
{
    [BindProperties(SupportsGet = true)]
    public class IndexModel : PageModel
    {
        private readonly ISiteRequestAccessor _siteRequestAccessor;

        public IndexModel(ISiteRequestAccessor siteRequestAccessor)
        {
            _siteRequestAccessor = siteRequestAccessor;
        }

        public FlexiblePageModel RenderPage { get; set; }

        public void OnGet()
        {
            RenderPage = _siteRequestAccessor.Current.RenderedPageModel;
        }
    }
}
