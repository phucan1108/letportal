using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Repositories.Apps;

namespace LetPortal.Portal.Providers.Apps
{
    public class InternalAppServiceProvider : IAppServiceProvider, IDisposable
    {
        private readonly IAppRepository _appRepository;

        public InternalAppServiceProvider(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareEntities(IEnumerable<App> apps)
        {
            var results = new List<ComparisonResult>();
            foreach (var app in apps)
            {
                results.Add(await _appRepository.Compare(app));
            }
            return results;
        }

        public async Task ForceUpdateApps(IEnumerable<App> apps)
        {
            foreach (var app in apps)
            {
                await _appRepository.ForceUpdateAsync(app.Id, app);
            }
        }

        public async Task<IEnumerable<App>> GetAppsByIds(IEnumerable<string> ids)
        {
            return await _appRepository.GetAllByIdsAsync(ids);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _appRepository.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
