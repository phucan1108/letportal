using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Recoveries;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Recoveries
{
    public class BackupEFRepository : EFGenericRepository<Backup>, IBackupRepository
    {
        private readonly DbContext _context;

        public BackupEFRepository(DbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
