using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public class SiteMongoRepository : MongoGenericRepository<Site>, ISiteRepository
    {
        public SiteMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<Site> GetByDomain(string domain)
        {
            var arrayDomainFilter = Builders<Site>.Filter.AnyEq(a => a.Domains, domain);
            var result = await Collection.FindAsync(arrayDomainFilter).ConfigureAwait(false);
            return result.FirstOrDefault();
        }
    }
}
