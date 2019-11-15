using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Repositories.Implements
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
