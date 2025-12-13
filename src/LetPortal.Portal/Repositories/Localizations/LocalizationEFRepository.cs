using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Repositories.Localizations
{
    public class LocalizationEFRepository : EFGenericRepository<Localization>, ILocalizationRepository
    {
        private readonly PortalDbContext _context;

        public LocalizationEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public override async Task<Localization> GetOneAsync(string id)
        {
            return await _context.Localizations
                        .Include(a => a.LocalizationContents)
                        .AsNoTracking()
                        .FirstAsync(a => a.Id == id);
        }

        public async Task<Localization> GetByLocaleId(string localeId, string appId)
        {
            return await _context
                .Localizations
                .AsNoTracking()
                .Include(a => a.LocalizationContents)
                .FirstOrDefaultAsync(b => b.LocaleId == localeId && b.AppId == appId);
        }

        public async Task<bool> CheckLocaleExisted(string localeId, string appId)
        {
            return await _context.Localizations.AnyAsync(a => a.LocaleId == localeId && a.AppId == appId);
        }

        public async Task DeleteByLocaleId(string localeId)
        {
            var existedLocale = await _context.Localizations.AsNoTracking().FirstAsync(a => a.LocaleId == localeId);
            await DeleteAsync(existedLocale.Id);
        }

        public async Task<IEnumerable<LanguageKey>> GetAppLangues(string appId, string localeId)
        {
            var keys = await _context
                                    .Localizations
                                    .Include(a =>
                                        a.LocalizationContents
                                            .Where(b => b.Key.StartsWith("apps")))
                                    .AsNoTracking()
                                    .AsQueryable()
                                    .Where(a => a.AppId == appId)
                                    .SelectMany(a => a.LocalizationContents)
                                    .Select(b => new LanguageKey { Key = b.Key, Value = b.Text })
                                    .ToListAsync();
            if(keys != null)
            {
                return keys;
            }
            else
            {
                return Enumerable.Empty<LanguageKey>();
            }
        }

        public async Task CloneLocalization(string appId, string cloningAppId)
        {
            var allLocales = await _context.Localizations.Include(a => a.LocalizationContents)
                                    .AsNoTracking()
                                    .AsQueryable()
                                    .Where(a => a.AppId == appId)
                                    .ToListAsync();
            if (allLocales != null)
            {
                foreach (var locale in allLocales)
                {
                    locale.Id = DataUtil.GenerateUniqueId();
                    locale.AppId = cloningAppId;
                    await AddAsync(locale);
                }
            }
        }

        public async Task DeleteAll(string appId)
        {
            var allLocales = await _context.Localizations.AsNoTracking().Where(a => a.AppId == appId).Select(b => b.Id).ToListAsync();

            if(allLocales != null && allLocales.Count > 0)
            {
                foreach(var locale in allLocales)
                {
                    await DeleteAsync(locale);
                }
            }
        }
    }
}
