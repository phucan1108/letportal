using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;

namespace LetPortal.Portal.Providers.Components
{
    public interface IChartServiceProvider
    {
        Task<IEnumerable<Chart>> GetChartsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareCharts(IEnumerable<Chart> charts);

        Task ForceUpdateCharts(IEnumerable<Chart> charts);

        Task<IEnumerable<Chart>> GetByAppId(string appId);

        Task DeleteByAppIdAsync(string appId);

        Task<bool> CheckChartExist(Expression<Func<Chart, bool>> expression);
    }
}
