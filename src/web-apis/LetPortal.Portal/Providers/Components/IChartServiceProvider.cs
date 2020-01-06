using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public interface IChartServiceProvider
    {
        Task<IEnumerable<Chart>> GetChartsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareCharts(IEnumerable<Chart> charts);

        Task ForceUpdateCharts(IEnumerable<Chart> charts);
    }
}
