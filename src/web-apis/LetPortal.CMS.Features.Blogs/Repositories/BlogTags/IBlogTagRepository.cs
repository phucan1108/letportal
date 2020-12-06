using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Features.Blogs.Repositories.BlogTags
{
    public interface IBlogTagRepository : IGenericRepository<BlogTag>
    {
        Task<IEnumerable<BlogTag>> GetTags(IEnumerable<string> tagIds);

        Task<IEnumerable<BlogTag>> GetTagsByName(IEnumerable<string> tags);
    }
}
