using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Features.Blogs.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.Blogs
{
    [ThemePart(BindingType = BindingType.None)]
    public class BlogCategoriesViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public BlogCategoriesViewComponent(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = (IEnumerable<Blog>)_siteRequest.Current.ResolvedData["blogs"];
            return Task.FromResult<IViewComponentResult>(View(blogs));
        }
    }
}
