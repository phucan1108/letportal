using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Shared;
using LetPortal.CMS.Features.Blogs.Entities;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models.Blogs;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.Blogs
{
    [ThemePart(BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None)]
    public class BlogCategoryViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public BlogCategoryViewComponent(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            var blog = (Blog)_siteRequest.Current.ResolvedData["blog"];
            var posts = (PaginationData<Post>)_siteRequest.Current.ResolvedData["posts"];
            var blogCateModel = new BlogCategoryModel
            {
                Blog = blog,
                Posts = posts
            };
            return Task.FromResult<IViewComponentResult>(View(blogCateModel));
        }
    }
}
