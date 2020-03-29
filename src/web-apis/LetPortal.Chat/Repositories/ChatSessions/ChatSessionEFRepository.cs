using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatSessions
{
    public class ChatSessionEFRepository : EFGenericRepository<ChatSession>, IChatSessionRepository
    {
        private readonly ChatDbContext _context;

        public ChatSessionEFRepository(ChatDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
