using System;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Chat.Repositories.ChatUsers
{
    public class ChatUserMongoRepository : MongoGenericRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task UpsertAsync(ChatUser chatUser)
        {
            var foundUser = await Collection.AsQueryable().FirstOrDefaultAsync(a => a.UserName == chatUser.UserName);
            if (foundUser == null)
            {
                chatUser.ActivatedDate = DateTime.UtcNow;
                chatUser.Deactivate = false;
                await AddAsync(chatUser);
            }
            else
            {
                foundUser.FullName = chatUser.FullName;
                foundUser.Avatar = chatUser.Avatar;
                foundUser.Deactivate = chatUser.Deactivate;
                if (foundUser.Deactivate)
                {
                    foundUser.DeactivatedDate = DateTime.UtcNow;
                }
                await UpdateAsync(foundUser.Id, foundUser);
            }
        }
    }
}
