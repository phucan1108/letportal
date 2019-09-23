using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

namespace LetPortal.ServiceManagement.Repositories
{
    public class MonitorCounterEFRepository : EFGenericRepository<MonitorCounter>, IMonitorCounterRepository
    {
        private readonly LetPortalServiceManagementDbContext _context;
        public MonitorCounterEFRepository(LetPortalServiceManagementDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
