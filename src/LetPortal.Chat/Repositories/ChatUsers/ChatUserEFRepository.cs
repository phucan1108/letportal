using System;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Chat.Repositories.ChatUsers
{
    public class ChatUserEFRepository : EFGenericRepository<ChatUser>, IChatUserRepository
    {
        private readonly ChatDbContext _context;

        public ChatUserEFRepository(ChatDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task UpsertAsync(ChatUser chatUser)
        {
            var foundUser = await _context.ChatUsers.AsQueryable().FirstOrDefaultAsync(a => a.UserName == chatUser.UserName);
            if (foundUser == null)
            {
                chatUser.Id = DataUtil.GenerateUniqueId();
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
