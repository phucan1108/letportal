using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Repositories.Blogs;

namespace LetPortal.CMS.Features.Blogs.Resolvers
{
    public class BlogsResolver : ISiteRouteResolver
    {
        private readonly IBlogRepository _blogRepository;

        public BlogsResolver(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public string Name => "blogs";

        public async Task Resolve(
            ISiteRequestAccessor request,
            Dictionary<string, string> routeValues,
            string setterKey)
        {
            var blogs = await _blogRepository.GetAllAsync();
            request.Current.ResolvedData.Add(setterKey, blogs);
        }
    }
}
