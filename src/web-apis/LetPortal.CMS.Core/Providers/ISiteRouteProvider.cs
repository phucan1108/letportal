using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Models;

namespace LetPortal.CMS.Core.Providers
{
    public interface ISiteRouteProvider
    {
        Task<SiteRouteMapCache> FindRouteAsync(string siteId, string localeId, string path);
    }
}
