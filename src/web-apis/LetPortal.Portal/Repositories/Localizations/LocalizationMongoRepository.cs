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

        public async Task<Localization> GetByPageIdAndLocaleId(string pageId, string localeId)
        {
            return await Collection.AsQueryable().FirstAsync(a => a.PageId == pageId && a.LocaleId == localeId);
        }
    }
}
