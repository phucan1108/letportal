using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Datasources;

namespace LetPortal.Portal.Repositories.Datasources
{
    public class DatasourceEFRepository : EFGenericRepository<Datasource>, IDatasourceRepository
    {
        private readonly LetPortalDbContext _context;

        public DatasourceEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
