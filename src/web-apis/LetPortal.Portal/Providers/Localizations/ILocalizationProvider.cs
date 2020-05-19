using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Providers.Localizations
{
    public interface ILocalizationProvider
    {
        Task<IEnumerable<LanguageKey>> CollectAlls(string appId);

        Task<IEnumerable<Localization>> GetByAppId(string appId);

        Task ForceUpdateLocalizations(IEnumerable<Localization> localizations);

        Task DeleteByAppIdAsync(string appId);

        Task<bool> CheckLocaleExist(Expression<Func<Localization, bool>> expression);
    }
}
