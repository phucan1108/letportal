using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatUsers
{
    public interface IChatUserRepository : IGenericRepository<ChatUser>
    {
        Task UpsertAsync(ChatUser chatUser);
    }
}
