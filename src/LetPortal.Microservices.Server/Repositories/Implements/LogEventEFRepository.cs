using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Repositories.Implements
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
