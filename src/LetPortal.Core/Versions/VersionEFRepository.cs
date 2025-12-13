using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Core.Versions
{
    public class VersionEFRepository : EFGenericRepository<Version>, IVersionRepository
    {
        public VersionEFRepository(DbContext context) : base(context)
        {
        }

        public async Task<Version> GetLastestVersion(string appName)
        {
            var versions = await GetAllAsync(a => a.AppName == appName);

            if(versions != null && versions.Any())
            {
                return versions.OrderByDescending(a => a.CreatedDate).First();
            }

            return null;
        }
    }
}
