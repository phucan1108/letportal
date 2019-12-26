using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public interface IDynamicListServiceProvider
    {
        Task<IEnumerable<DynamicList>> GetDynamicListsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareDynamicLists(IEnumerable<DynamicList> dynamicLists);
    }
}
