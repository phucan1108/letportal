using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Providers.Components
{
    public interface IDynamicListServiceProvider
    {
        Task<IEnumerable<DynamicList>> GetDynamicListsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareDynamicLists(IEnumerable<DynamicList> dynamicLists);

        Task ForceUpdateDynamicLists(IEnumerable<DynamicList> dynamicLists);

        Task<IEnumerable<DynamicList>> GetByAppId(string appId);

        Task DeleteByAppIdAsync(string appId);

        Task<bool> CheckDynamicListExist(Expression<Func<DynamicList, bool>> expression);
    }
}
