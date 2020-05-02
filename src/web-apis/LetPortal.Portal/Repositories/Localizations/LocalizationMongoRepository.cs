using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Portal.Repositories.Localizations
{
    public class LocalizationMongoRepository : MongoGenericRepository<Localization>, ILocalizationRepository
    {
        public LocalizationMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<bool> CheckLocaleExisted(string localeId)
        {
            return await Collection.AsQueryable().AnyAsync(a => a.LocaleId == localeId);
        }

        public async Task<Localization> GetByLocaleId(string localeId)
        {
            return await Collection.AsQueryable().FirstAsync(a => a.LocaleId == localeId);
        }
    }
}
