using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Core.Versions
{
    public class PatchVersionEFRepository : EFGenericRepository<PatchVersion>, IPatchVersionRepository
    {

        public PatchVersionEFRepository(DbContext context)
            :base(context)
        {
        }

        public async Task<PatchVersion> GetLatestAsync(string appName, string patchName)
        {
            var allVersions = await GetAllAsync(a => a.AppName == appName && a.PatchName == patchName);
            if (allVersions.Any())
            {
                return allVersions.Select(a => new { Version = a, VersionNumber = a.VersionNumber.ToVersionNumber() }).OrderByDescending(b => b.VersionNumber).First().Version;
            }
            return null;
        }
    }
}
