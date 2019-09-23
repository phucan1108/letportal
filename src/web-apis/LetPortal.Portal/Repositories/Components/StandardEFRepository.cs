using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Repositories.Components
{
    public class StandardEFRepository : EFGenericRepository<StandardComponent>, IStandardRepository
    {
        private readonly LetPortalDbContext _context;

        public StandardEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
