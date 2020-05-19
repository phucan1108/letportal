using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Repositories.Components;

namespace LetPortal.Portal.Providers.Components
{
    public class InternalDynamicListServiceProvider : IDynamicListServiceProvider, IDisposable
    {
        private readonly IDynamicListRepository _dynamicListRepository;

        public InternalDynamicListServiceProvider(IDynamicListRepository dynamicListRepository)
        {
            _dynamicListRepository = dynamicListRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareDynamicLists(IEnumerable<DynamicList> dynamicLists)
        {
            var results = new List<ComparisonResult>();
            foreach (var dynamicList in dynamicLists)
            {
                results.Add(await _dynamicListRepository.Compare(dynamicList));
            }
            return results;
        }

        public async Task ForceUpdateDynamicLists(IEnumerable<DynamicList> dynamicLists)
        {
            foreach (var dynamicList in dynamicLists)
            {
                await _dynamicListRepository.ForceUpdateAsync(dynamicList.Id, dynamicList);
            }
        }

        public async Task<IEnumerable<DynamicList>> GetDynamicListsByIds(IEnumerable<string> ids)
        {
            return await _dynamicListRepository.GetAllByIdsAsync(ids);
        }

        public async Task<IEnumerable<DynamicList>> GetByAppId(string appId)
        {
            return await _dynamicListRepository.GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);
        }  

        public async Task DeleteByAppIdAsync(string appId)
        {
            var allLists = await _dynamicListRepository.GetAllAsync(a => a.AppId == appId);
            if(allLists != null && allLists.Any())
            {
                foreach(var list in allLists)
                {
                    await _dynamicListRepository.DeleteAsync(list.Id);
                }
            }
        }

        public async Task<bool> CheckDynamicListExist(Expression<Func<DynamicList, bool>> expression)
        {
            return await _dynamicListRepository.IsExistAsync(expression);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dynamicListRepository.Dispose();
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
