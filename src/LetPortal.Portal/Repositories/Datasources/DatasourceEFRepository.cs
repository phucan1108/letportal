using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Datasources;

namespace LetPortal.Portal.Repositories.Datasources
{
    public class DatasourceEFRepository : EFGenericRepository<Datasource>, IDatasourceRepository
    {
        private readonly PortalDbContext _context;

        public DatasourceEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
