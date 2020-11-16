using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Core.Versions
{
    public class PatchVersionMongoRepository : MongoGenericRepository<PatchVersion>, IPatchVersionRepository
    {
        public PatchVersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<PatchVersion> GetLatestAsync(string appName, string patchName)
        {
            var allVersions = await Collection.AsQueryable().Where(a => a.AppName == appName && a.PatchName == patchName).ToListAsync();
            if(allVersions.Count > 0)
            {
                return allVersions.Select(a => new { Version = a, VersionNumber = a.VersionNumber.ToVersionNumber() }).OrderByDescending(b => b.VersionNumber).First().Version;
            }
            return null;
        }
    }
}
