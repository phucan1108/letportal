using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;

namespace LetPortal.Portal.Repositories.Localizations
{
    public interface ILocalizationRepository : IGenericRepository<Localization>
    {
        Task<Localization> GetByPageIdAndLocaleId(string pageId, string localeId);
    }
}
