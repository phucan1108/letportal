using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Versions;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.PortalVersions
{
    public class PortalVersionEFRepository : EFGenericRepository<PortalVersion>, IPortalVersionRepository
    {
        private readonly DbContext _context;

        public PortalVersionEFRepository(DbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
