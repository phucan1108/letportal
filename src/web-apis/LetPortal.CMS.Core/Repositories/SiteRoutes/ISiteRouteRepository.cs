using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.SiteRoutes
{
    public interface ISiteRouteRepository : IGenericRepository<SiteRoute>
    {
        Task<IEnumerable<SiteRoute>> GetRoutesSiteAsync(string siteId);
    }
}
