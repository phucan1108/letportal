using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Repositories.Components;

namespace LetPortal.Portal.Providers.Components
{
    public class InternalChartServiceProvider : IChartServiceProvider, IDisposable
    {
        private readonly IChartRepository _chartRepository;

        public InternalChartServiceProvider(IChartRepository chartRepository)
        {
            _chartRepository = chartRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareCharts(IEnumerable<Chart> charts)
        {
            var results = new List<ComparisonResult>();
            foreach (var chart in charts)
            {
                results.Add(await _chartRepository.Compare(chart));
            }
            return results;
        }

        public async Task ForceUpdateCharts(IEnumerable<Chart> charts)
        {
            foreach (var chart in charts)
            {
                await _chartRepository.ForceUpdateAsync(chart.Id, chart);
            }
        }

        public async Task<IEnumerable<Chart>> GetChartsByIds(IEnumerable<string> ids)
        {
            return await _chartRepository.GetAllByIdsAsync(ids);
        }

        public async Task<IEnumerable<Chart>> GetByAppId(string appId)
        {
            return await _chartRepository.GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);
        }

        public async Task DeleteByAppIdAsync(string appId)
        {
            var allCharts = await _chartRepository.GetAllAsync(a => a.AppId == appId);

            if(allCharts != null && allCharts.Any())
            {
                foreach(var chart in allCharts)
                {
                    await _chartRepository.DeleteAsync(chart.Id);
                }
            }
        }

        public async Task<bool> CheckChartExist(Expression<Func<Chart, bool>> expression)
        {
            return await _chartRepository.IsExistAsync(expression);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _chartRepository.Dispose();
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
