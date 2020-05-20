using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Repositories.Localizations
{
    public interface ILocalizationRepository : IGenericRepository<Localization>
    {
        Task<IEnumerable<LanguageKey>> GetAppLangues(string appId, string localeId);

        Task<Localization> GetByLocaleId(string localeId, string appId);

        Task<bool> CheckLocaleExisted(string localeId, string appId);

        Task DeleteByLocaleId(string localeId);

        Task DeleteAll(string appId);

        Task CloneLocalization(string appId, string cloningAppId);
    }
}
