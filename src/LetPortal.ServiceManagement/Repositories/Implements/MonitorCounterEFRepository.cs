using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Repositories.Implements
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
