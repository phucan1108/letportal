using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IChartRepository : IGenericRepository<Chart>
    {
        Task CloneAsync(string cloneId, string cloneName);

        Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null);

        Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string chartId);

        Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId);
    }
}
