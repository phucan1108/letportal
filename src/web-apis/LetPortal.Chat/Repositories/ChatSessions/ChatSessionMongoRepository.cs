using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatSessions
{
    public class ChatSessionMongoRepository : MongoGenericRepository<ChatSession>, IChatSessionRepository
    {
        public ChatSessionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
