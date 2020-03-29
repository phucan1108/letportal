using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Databases
{
    public class DatabaseEFRepository : EFGenericRepository<DatabaseConnection>, IDatabaseRepository
    {
        private readonly PortalDbContext _context;

        public DatabaseEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneDatabase = await _context.Databases.AsNoTracking().FirstAsync(a => a.Id == cloneId);
            cloneDatabase.Id = DataUtil.GenerateUniqueId();
            cloneDatabase.Name = cloneName;
            cloneDatabase.DisplayName += " Clone";
            await AddAsync(cloneDatabase);
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortDatatabases(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var shortDatabases = await _context.Databases.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();
                return shortDatabases?.AsEnumerable();
            }
            else
            {
                return (await _context.Databases.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync())?.AsEnumerable();
            }
        }
    }
}
