using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Repositories.Posts;

namespace LetPortal.CMS.Features.Blogs.Resolvers
{
    public class PostResolver : ISiteRouteResolver
    {
        private readonly IPostRepository _postRepository;

        const string POST_KEY = "blogPost";

        public PostResolver(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public string Name => "blogPost";

        public async Task Resolve(
            ISiteRequestAccessor request,
            Dictionary<string, string> routeValues,
            string setterKey)
        {
            var postValue = routeValues[POST_KEY];
            var post = await _postRepository.GetByUrlPathAsync(postValue);
            request.Current.ResolvedData.Add(setterKey, post);
        }
    }
}
