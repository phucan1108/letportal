using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Repositories.Blogs;

namespace LetPortal.CMS.Features.Blogs.Resolvers
{
    public class BlogResolver : ISiteRouteResolver
    {
        private readonly IBlogRepository _blogRepository;

        const string BLOG_KEY = "blog";

        public BlogResolver(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public string Name => "blog";

        public async Task Resolve(
            ISiteRequestAccessor request,
            Dictionary<string, string> routeValues,
            string setterKey)
        {
            var blogUrlPath = routeValues[BLOG_KEY];

            var blog = await _blogRepository.GetByUrlPathAsync(blogUrlPath);
            request.Current.ResolvedData.Add(setterKey, blog);
        }
    }
}
