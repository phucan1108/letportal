using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public class UserSessionEFRepository : EFGenericRepository<UserSession>, IUserSessionRepository
    {
        private readonly IdentityDbContext _context;

        public UserSessionEFRepository(IdentityDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task LogUserActivityAsync(string userSessionId, UserActivity userActivity)
        {
            var userSession = _context.UserSessions.First(a => a.Id == userSessionId);

            if (userSession.UserActivities == null)
            {
                userSession.UserActivities = new List<UserActivity>();
            }
            userSession.UserActivities.Add(userActivity);

            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
