using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Localizations;
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

        private readonly ILocalizationRepository _localizationRepository;

        public InternalLocalizationProvider(
            IPageRepository pageRepository,
            IStandardRepository standardRepository,
            IChartRepository chartRepository,
            IDynamicListRepository dynamicListRepository,
            IAppRepository appRepository,
            ILocalizationRepository localizationRepository)
        {
            _pageRepository = pageRepository;
            _standardRepository = standardRepository;
            _chartRepository = chartRepository;
            _dynamicListRepository = dynamicListRepository;
            _appRepository = appRepository;
            _localizationRepository = localizationRepository;
        }

        public async Task<bool> CheckLocaleExist(Expression<Func<Localization, bool>> expression)
        {
            return await _localizationRepository.IsExistAsync(expression);
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

        public async Task DeleteByAppIdAsync(string appId)
        {
            var allLocalizations = await _localizationRepository.GetAllAsync(a => a.AppId == appId);

            if(allLocalizations != null && allLocalizations.Any())
            {
                foreach(var locale in allLocalizations)
                {
                    await _localizationRepository.DeleteAsync(locale.Id);
                }
            }
        }

        public async Task ForceUpdateLocalizations(IEnumerable<Localization> localizations)
        {
            foreach(var localization in localizations)
            {
                await _localizationRepository.ForceUpdateAsync(localization.Id, localization);
            }
        }

        public async Task<IEnumerable<Localization>> GetByAppId(string appId)
        {
            return await _localizationRepository.GetAllAsync(a => a.AppId == appId);
        }
    }
}
