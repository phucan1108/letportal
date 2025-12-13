using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Features.Blogs.Repositories.Blogs
{
    public interface IBlogRepository : IGenericRepository<Blog>
    {
        Task<Blog> GetByUrlPathAsync(string urlPath);
    }
}
