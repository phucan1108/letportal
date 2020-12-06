using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.CMS.Features.Blogs.Repositories.Blogs;
using LetPortal.CMS.Features.Blogs.Repositories.BlogTags;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models.Blogs;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.Blogs
{
    [ThemePart(BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None)]
    public class BlogPostViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IBlogTagRepository _blogTagRepository;

        private readonly IBlogRepository _blogRepository;

        public BlogPostViewComponent(
            ISiteRequestAccessor siteRequest,
            IBlogRepository blogRepository,
            IBlogTagRepository blogTagRepository)
        {
            _siteRequest = siteRequest;
            _blogTagRepository = blogTagRepository;
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var post = (Post)_siteRequest.Current.ResolvedData["blogPost"];
            var blog = await _blogRepository.GetOneAsync(post.BlogId);
            var model = new BlogPostModel
            {
                Blog = blog,
                Post = post
            };
            if (post.Tags != null && post.Tags.Any())
            {
                var tags = await _blogTagRepository.GetTags(post.Tags);
                model.Tags = tags;
            }

            return View(model);
        }
    }
}
