using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Driver;

namespace LetPortal.Core.Versions
{
    public class VersionMongoRepository : MongoGenericRepository<Version>, IVersionRepository
    {
        public VersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public Task<Version> GetLastestVersion(string appName)
        {
            var versions = Collection.AsQueryable().Where(a => a.AppName == appName).ToList();

            if(versions != null && versions.Any())
            {
                return Task.FromResult(versions.OrderByDescending(a => a.CreatedDate).First());
            }

            return Task.FromResult(default(Version));
        }
    }
}
