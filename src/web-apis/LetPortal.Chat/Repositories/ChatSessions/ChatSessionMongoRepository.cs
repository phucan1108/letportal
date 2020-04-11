using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Chat.Repositories.ChatSessions
{
    public class ChatSessionMongoRepository : MongoGenericRepository<ChatSession>, IChatSessionRepository
    {
        public ChatSessionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<ChatSession> GetFullSessionById(string chatSessionId)
        {
            return await GetOneAsync(chatSessionId);
        }

        public async Task<ChatSession> GetLastChatSession(string chatRoomId)
        {
            return await Collection.AsQueryable().Where(a => a.ChatRoomId == chatRoomId).OrderByDescending(b => b.CreatedDate).FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(ChatSession chatSession)
        {
            var found = await Collection.AsQueryable().FirstOrDefaultAsync(a => a.Id == chatSession.Id);
            if (found == null)
            {                   
                await AddAsync(chatSession);
            }
            else
            {                  
                await UpdateAsync(found.Id, chatSession);
            }
        }
    }
}
