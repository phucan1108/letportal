using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Repositories.Implements
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
