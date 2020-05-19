using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Pages;

namespace LetPortal.Portal.Providers.Pages
{
    public class InternalPageServiceProvider : IPageServiceProvider, IDisposable
    {
        private readonly IPageRepository _pageRepository;

        private readonly IStandardRepository _standardRepository;

        private readonly IChartRepository _chartRepository;

        private readonly IDynamicListRepository _dynamicListRepository;

        public InternalPageServiceProvider(
            IPageRepository pageRepository,
            IStandardRepository standardRepository,
            IChartRepository chartRepository,
            IDynamicListRepository dynamicListRepository)
        {
            _pageRepository = pageRepository;
            _standardRepository = standardRepository;
            _chartRepository = chartRepository;
            _dynamicListRepository = dynamicListRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> ComparePages(IEnumerable<Page> pages)
        {
            var results = new List<ComparisonResult>();
            foreach (var page in pages)
            {
                results.Add(await _pageRepository.Compare(page));
            }
            return results;
        }

        public async Task ForceUpdatePages(IEnumerable<Page> pages)
        {
            foreach (var page in pages)
            {
                await _pageRepository.ForceUpdateAsync(page.Id, page);
            }
        }

        public async Task<List<ShortPageModel>> GetAllPages()
        {
            return await _pageRepository.GetAllShortPagesAsync();
        }

        public async Task<IEnumerable<Page>> GetPagesByIds(IEnumerable<string> ids)
        {
            return await _pageRepository.GetAllByIdsAsync(ids);
        }

        public async Task<IEnumerable<LanguageKey>> GetPageLanguages(string pageId)
        {
            var languages = new List<LanguageKey>();
            var page = await _pageRepository.GetOneAsync(pageId);

            var pageLanguages = await _pageRepository.GetLanguageKeys(pageId);
            languages.AddRange(pageLanguages);
            if (page.Builder != null && page.Builder.Sections != null)
            {

                foreach (var section in page.Builder.Sections)
                {
                    switch (section.ConstructionType)
                    {
                        case SectionContructionType.Standard:
                            var standardLanguages = await _standardRepository.GetLanguageKeysAsync(section.ComponentId);
                            languages.AddRange(standardLanguages?.ToList());
                            break;
                        case SectionContructionType.Array:
                            var standardArrayLanguages = await _standardRepository.GetLanguageKeysAsync(section.ComponentId);
                            languages.AddRange(standardArrayLanguages.ToList());
                            break;
                        case SectionContructionType.DynamicList:
                            var dynamicListLanguages = await _dynamicListRepository.GetLanguageKeysAsync(section.ComponentId);
                            languages.AddRange(dynamicListLanguages.ToList());
                            break;
                        case SectionContructionType.Chart:
                            var chartLanguages = await _chartRepository.GetLanguageKeysAsync(section.ComponentId);
                            languages.AddRange(chartLanguages.ToList());
                            break;
                    }
                }

            }
            return languages;
        }

        public async Task<IEnumerable<Page>> GetByAppId(string appId)
        {
            return await _pageRepository.GetAllAsync(a => a.AppId == appId);
        }

        public async Task DeleteByAppIdAsync(string appId)
        {
            var allPages = await _pageRepository.GetAllAsync(a => a.AppId == appId);

            if(allPages != null && allPages.Any())
            {
                foreach(var page in allPages)
                {
                    await _pageRepository.DeleteAsync(page.Id);
                }
            }
        }

        public async Task<bool> CheckPageExist(Expression<Func<Page, bool>> expression)
        {
            return await _pageRepository.IsExistAsync(expression);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pageRepository.Dispose();
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
