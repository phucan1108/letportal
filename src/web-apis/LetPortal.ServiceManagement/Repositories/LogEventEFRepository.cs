using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

namespace LetPortal.ServiceManagement.Repositories
{
    public class LogEventEFRepository : EFGenericRepository<LogEvent>, ILogEventRepository
    {
        private readonly LetPortalServiceManagementDbContext _context;
        public LogEventEFRepository(LetPortalServiceManagementDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
