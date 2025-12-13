using System.Threading.Tasks;
using LetPortal.CMS.Features.Menus.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Features.Menus.Repositories
{
    public interface ISiteMenuRepository : IGenericRepository<SiteMenu>
    {
        Task<SiteMenu> GetBySiteId(string siteId);  
    }
}
