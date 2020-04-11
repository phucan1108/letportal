using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatSessions
{
    public interface IChatSessionRepository : IGenericRepository<ChatSession>
    {
        Task UpsertAsync(ChatSession chatSession);

        Task<ChatSession> GetLastChatSession(string chatRoomId);

        Task<ChatSession> GetFullSessionById(string chatSessionId);
    }
}
