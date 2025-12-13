using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.Chat.Repositories.ChatRooms
{
    public class ChatRoomEFRepository : EFGenericRepository<ChatRoom>, IChatRoomRepository
    {
        private readonly ChatDbContext _context;

        public ChatRoomEFRepository(ChatDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
