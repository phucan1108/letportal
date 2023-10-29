using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.MessageGroups
{
    public interface IMessageGroupRepository : IGenericRepository<MessageGroup>
    {
        Task UpdateLastClicked(string subcriberId, string messageGroupId, long lastClickedTs);
    }
}
