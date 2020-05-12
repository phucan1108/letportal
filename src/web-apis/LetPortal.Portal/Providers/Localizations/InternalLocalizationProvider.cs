using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Pages;

namespace LetPortal.Portal.Providers.Localizations
{
    public class InternalLocalizationProvider : ILocalizationProvider
    {
        private readonly IPageRepository _pageRepository;

        private readonly IStandardRepository _standardRepository;

        private readonly IChartRepository _chartRepository;

        private readonly IDynamicListRepository _dynamicListRepository;

        private readonly IAppRepository _appRepository;

        public InternalLocalizationProvider(
            IPageRepository pageRepository,
            IStandardRepository standardRepository,
            IChartRepository chartRepository,
            IDynamicListRepository dynamicListRepository,
            IAppRepository appRepository)
        {
            _pageRepository = pageRepository;
            _standardRepository = standardRepository;
            _chartRepository = chartRepository;
            _dynamicListRepository = dynamicListRepository;
            _appRepository = appRepository;
        }

        public async Task<IEnumerable<LanguageKey>> CollectAlls(string appId)
        {
            var languages = new List<LanguageKey>();

            languages.AddRange(await _appRepository.CollectAllLanguages(appId));
            languages.AddRange(await _standardRepository.CollectAllLanguages(appId));
            languages.AddRange(await _dynamicListRepository.CollectAllLanguages(appId));
            languages.AddRange(await _chartRepository.CollectAllLanguages(appId));
            languages.AddRange(await _pageRepository.CollectAllLanguages(appId));

            return languages;
        }
    }
}
