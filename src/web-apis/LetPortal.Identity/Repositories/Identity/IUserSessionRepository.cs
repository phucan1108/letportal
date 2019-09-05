using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using System.Threading.Tasks;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IUserSessionRepository : IGenericRepository<UserSession>
    {
        Task LogUserActivityAsync(string userSessionId, UserActivity userActivity);
    }
}
