using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatRooms
{
    public class ChatRoomMongoRepository : MongoGenericRepository<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
