using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;
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

        public async Task<bool> CheckLocaleExisted(string localeId, string appId)
        {
            return await Collection
                    .AsQueryable()
                    .AnyAsync(a => a.LocaleId == localeId && a.AppId == appId);
        }

        public async Task CloneLocalization(string appId, string cloningAppId)
        {
            var allLocales = await Collection.AsQueryable().Where(a => a.AppId == appId).ToListAsync();
            if(allLocales != null)
            {
                foreach(var locale in allLocales)
                {
                    locale.Id = DataUtil.GenerateUniqueId();
                    locale.AppId = cloningAppId;
                    await AddAsync(locale);
                }
            }
        }

        public async Task DeleteAll(string appId)
        {
            await Collection.DeleteManyAsync(a => a.AppId == appId);
        }

        public async Task DeleteByLocaleId(string localeId)
        {
            var existedLocale = await Collection.AsQueryable().FirstAsync(a => a.LocaleId == localeId);
            await DeleteAsync(existedLocale.Id);
        }

        public async Task<IEnumerable<LanguageKey>> GetAppLangues(string appId, string localeId)
        {
            var localization = await Collection.AsQueryable().FirstOrDefaultAsync(a => a.AppId == appId && a.LocaleId == localeId);

            if(localization != null)
            {
                return localization.LocalizationContents.Where(b => b.Key.StartsWith("apps")).Select(a => new LanguageKey { Key = a.Key, Value = a.Text });
            }
            else
            {
                return Enumerable.Empty<LanguageKey>();
            }               
        }

        public async Task<Localization> GetByLocaleId(string localeId, string appId)
        {
            return await Collection
                    .AsQueryable()
                    .FirstOrDefaultAsync(a => a.LocaleId == localeId && a.AppId == appId);
        }
    }
}
