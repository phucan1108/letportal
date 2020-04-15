using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ChatSession> GetFullSessionById(string chatSessionId)
        {
            return await _context.ChatSessions
                            .AsNoTracking()
                            .Include(b => b.Conversations)
                            .FirstOrDefaultAsync(a => a.Id == chatSessionId);
        }

        public async Task<ChatSession> GetLastChatSession(string chatRoomId)
        {
            return await _context.ChatSessions
                .AsNoTracking()
                .Include(b => b.Conversations)
                .Where(a => a.ChatRoomId == chatRoomId).OrderByDescending(b => b.CreatedDate).FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(ChatSession chatSession)
        {
            var found = await _context.ChatSessions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == chatSession.Id);
            if (found != null)
            {
                await DeleteAsync(found.Id);
            }

            if (string.IsNullOrEmpty(chatSession.Id))
            {
                chatSession.Id = DataUtil.GenerateUniqueId();
            }
            if (chatSession.Conversations != null)
            {
                foreach (var conversation in chatSession.Conversations)
                {
                    if (string.IsNullOrEmpty(conversation.Id))
                    {
                        conversation.Id = DataUtil.GenerateUniqueId();
                    }

                    conversation.ChatSessionId = chatSession.Id;
                }
            }
            await AddAsync(chatSession);

        }
    }
}
