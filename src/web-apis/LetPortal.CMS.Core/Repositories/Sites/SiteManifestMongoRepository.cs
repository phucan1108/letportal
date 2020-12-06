using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public class SiteManifestMongoRepository : MongoGenericRepository<SiteManifest>, ISiteManifestRepository
    {
        public SiteManifestMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<List<SiteManifest>> GetSiteManifestsAsync(
            IEnumerable<string> keys,
            string siteId)
        {
            var arrayFilter = Builders<SiteManifest>.Filter.In(a => a.Key, keys);
            var siteFilter = Builders<SiteManifest>.Filter.Eq(a => a.SiteId, siteId);
            var andFilter = Builders<SiteManifest>.Filter.And(siteFilter, arrayFilter);
            var result = await Collection.FindAsync(andFilter);
            return await result.ToListAsync();
        }
    }
}
