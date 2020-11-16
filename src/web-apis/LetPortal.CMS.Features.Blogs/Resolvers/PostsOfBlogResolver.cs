using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.CMS.Features.Blogs.Repositories.Posts;

namespace LetPortal.CMS.Features.Blogs.Resolvers
{
    public class PostsOfBlogResolver : ISiteRouteResolver
    {
        public string Name => "postsOfBlog";

        private readonly IPostRepository _postRepository;

        const string BLOG_KEY = "blog";

        public PostsOfBlogResolver(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task Resolve(ISiteRequestAccessor request, Dictionary<string, string> routeValues, string setterKey)
        {
            var blog = (Blog)request.Current.ResolvedData[BLOG_KEY];

            var posts = await _postRepository.GetPostsByBlogIdAndPaginationAsync(blog.Id, request.Current.Pagination);

            request.Current.ResolvedData.Add(setterKey, posts);
        }
    }
}
