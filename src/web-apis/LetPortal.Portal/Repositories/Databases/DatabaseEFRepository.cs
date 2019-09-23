using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;

namespace LetPortal.Portal.Repositories.Databases
{
    public class DatabaseEFRepository : EFGenericRepository<DatabaseConnection>, IDatabaseRepository
    {
        private readonly LetPortalDbContext _context;

        public DatabaseEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
