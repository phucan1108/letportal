using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Repositories.BlogTags;
using LetPortal.CMS.Features.Blogs.Repositories.Posts;

namespace LetPortal.CMS.Features.Blogs.Resolvers
{
    public class PostsOfTagResolver : ISiteRouteResolver
    {
        private readonly IPostRepository _postRepository;

        private readonly IBlogTagRepository _blogTagRepository;

        public PostsOfTagResolver(IPostRepository postRepository, IBlogTagRepository blogTagRepository)
        {
            _postRepository = postRepository;
            _blogTagRepository = blogTagRepository;
        }

        public string Name => "postsOfTag";

        public async Task Resolve(ISiteRequestAccessor request, Dictionary<string, string> routeValues, string setterKey)
        {
            var tags = request.Current.QueryParams["tags"].Split("+");
            var tagEntities = await _blogTagRepository.GetTagsByName(tags.AsEnumerable());

            if(tagEntities != null && tagEntities.Any())
            {
                var posts = await _postRepository.GetPostsByTags(tagEntities.Select(a => a.Id), request.Current.Pagination);
                request.Current.ResolvedData.Add(setterKey, posts);
            }
        }
    }
}
