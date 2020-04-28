using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Localizations;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Localization> GetByPageIdAndLocaleId(string pageId, string localeId)
        {
            return await _context
                .Localizations
                .AsNoTracking()
                .Include(a => a.LocalizationContents)
                .FirstAsync(b => b.PageId == pageId && b.LocaleId == localeId);
        }
    }
}
