using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;

namespace LetPortal.Portal.Repositories.Localizations
{
    public interface ILocalizationRepository : IGenericRepository<Localization>
    {
        Task<Localization> GetByLocaleId(string localeId);

        Task<bool> CheckLocaleExisted(string localeId);

        Task DeleteByLocaleId(string localeId);
    }
}
