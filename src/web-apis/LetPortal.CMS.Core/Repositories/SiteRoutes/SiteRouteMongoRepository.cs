using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.CMS.Core.Repositories.SiteRoutes
{
    public class SiteRouteMongoRepository : MongoGenericRepository<SiteRoute>, ISiteRouteRepository
    {
        public SiteRouteMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<IEnumerable<SiteRoute>> GetRoutesSiteAsync(string siteId)
        {
            return await Collection.AsQueryable().Where(a => a.SiteId == siteId).ToListAsync();
        }
    }
}
