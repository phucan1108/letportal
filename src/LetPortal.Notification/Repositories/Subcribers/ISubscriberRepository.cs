using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.Subcribers
{
    public interface ISubscriberRepository : IGenericRepository<Subcriber>
    {
        Task<IEnumerable<Subcriber>> GetByRoles(string[] roles);

        Task UpdateLastClicked(string subcriberId, long lastClickedTs);
    }
}
