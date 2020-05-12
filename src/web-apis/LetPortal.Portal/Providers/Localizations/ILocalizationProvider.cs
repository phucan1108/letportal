using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Providers.Localizations
{
    public interface ILocalizationProvider
    {
        Task<IEnumerable<LanguageKey>> CollectAlls(string appId);
    }
}
