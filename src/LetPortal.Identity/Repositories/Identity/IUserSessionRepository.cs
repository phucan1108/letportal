using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IUserSessionRepository : IGenericRepository<UserSession>
    {
        Task LogUserActivityAsync(string userSessionId, UserActivity userActivity);
    }
}
