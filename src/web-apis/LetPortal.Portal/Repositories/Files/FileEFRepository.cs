using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Files;

namespace LetPortal.Portal.Repositories.Files
{
    public class FileEFRepository : EFGenericRepository<File>, IFileRepository
    {
        private readonly LetPortalDbContext _context;

        public FileEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
