using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public interface ISiteRepository : IGenericRepository<Site>
    {
        Task<Site> GetByDomain(string domain);
    }
}
