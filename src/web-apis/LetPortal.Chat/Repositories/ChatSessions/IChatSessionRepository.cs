using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatSessions
{
    public interface IChatSessionRepository : IGenericRepository<ChatSession>
    {
    }
}
