using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartEFRepository : EFGenericRepository<Chart>, IChartRepository
    {
        private readonly LetPortalDbContext _context;

        public ChartEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
