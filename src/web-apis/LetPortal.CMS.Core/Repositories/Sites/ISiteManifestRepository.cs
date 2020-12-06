using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public interface ISiteManifestRepository : IGenericRepository<SiteManifest>
    {
        Task<List<SiteManifest>> GetSiteManifestsAsync(IEnumerable<string> keys, string siteId);
    }
}
