using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.Channels
{
    public class ChannelMongoRepository : MongoGenericRepository<Channel>, IChannelRepository
    {
        public ChannelMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
