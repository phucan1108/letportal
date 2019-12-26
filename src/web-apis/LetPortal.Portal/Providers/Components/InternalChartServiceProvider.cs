using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Repositories.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public class InternalChartServiceProvider : IChartServiceProvider
    {
        private readonly IChartRepository _chartRepository;

        public InternalChartServiceProvider(IChartRepository chartRepository)
        {
            _chartRepository = chartRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareCharts(IEnumerable<Chart> charts)
        {
            var results = new List<ComparisonResult>();
            foreach(var chart in charts)
            {
                results.Add(await _chartRepository.Compare(chart));
            }
            return results;
        }

        public async Task<IEnumerable<Chart>> GetChartsByIds(IEnumerable<string> ids)
        {
            return await _chartRepository.GetAllByIdsAsync(ids);
        }
    }
}
