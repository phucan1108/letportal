using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Files;

namespace LetPortal.Portal.Repositories.Files
{
    public class FileEFRepository : EFGenericRepository<File>, IFileRepository
    {
        private readonly PortalDbContext _context;

        public FileEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
