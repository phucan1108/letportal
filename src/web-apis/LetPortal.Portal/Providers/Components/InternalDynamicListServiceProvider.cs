using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Repositories.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public class InternalDynamicListServiceProvider : IDynamicListServiceProvider
    {
        private readonly IDynamicListRepository _dynamicListRepository;

        public InternalDynamicListServiceProvider(IDynamicListRepository dynamicListRepository)
        {
            _dynamicListRepository = dynamicListRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareDynamicLists(IEnumerable<DynamicList> dynamicLists)
        {
            var results = new List<ComparisonResult>();
            foreach(var dynamicList in dynamicLists)
            {
                results.Add(await _dynamicListRepository.Compare(dynamicList));
            }
            return results;
        }

        public async Task ForceUpdateDynamicLists(IEnumerable<DynamicList> dynamicLists)
        {
            foreach(var dynamicList in dynamicLists)
            {
                await _dynamicListRepository.ForceUpdateAsync(dynamicList.Id, dynamicList);
            }
        }

        public async Task<IEnumerable<DynamicList>> GetDynamicListsByIds(IEnumerable<string> ids)
        {
            return await _dynamicListRepository.GetAllByIdsAsync(ids);
        }
    }
}
