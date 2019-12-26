using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Repositories.Apps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Apps
{
    public class InternalAppServiceProvider : IAppServiceProvider
    {
        private readonly IAppRepository _appRepository;

        public InternalAppServiceProvider(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareEntities(IEnumerable<App> apps)
        {
            var results = new List<ComparisonResult>();
            foreach(var app in apps)
            {
                results.Add(await _appRepository.Compare(app));
            }
            return results;
        }

        public async Task<IEnumerable<App>> GetAppsByIds(IEnumerable<string> ids)
        {
            return await _appRepository.GetAllByIdsAsync(ids);
        }
    }
}
