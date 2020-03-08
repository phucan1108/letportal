using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Providers.Apps
{
    public interface IAppServiceProvider
    {
        Task<IEnumerable<App>> GetAppsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareEntities(IEnumerable<App> apps);

        Task ForceUpdateApps(IEnumerable<App> apps);
    }
}
