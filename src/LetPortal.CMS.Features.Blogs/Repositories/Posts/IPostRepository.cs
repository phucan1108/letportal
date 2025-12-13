using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Shared;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Features.Blogs.Repositories.Posts
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post> GetByUrlPathAsync(string urlPath);

        Task<PaginationData<Post>> GetPostsByBlogIdAndPaginationAsync(string blogId, Pagination pagination, bool ascending = false);

        Task<PaginationData<Post>> GetPostsByTags(IEnumerable<string> tags, Pagination pagination, bool ascending = false);
    }
}
