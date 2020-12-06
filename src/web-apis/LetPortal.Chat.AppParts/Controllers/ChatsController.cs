using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Chat.Hubs;
using LetPortal.Chat.Models;
using LetPortal.Chat.Repositories.ChatUsers;
using LetPortal.Core.Https;
using LetPortal.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LetPortal.Chat.AppParts.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHubContext<HubChatClient, IHubChatClient> _hubChatContext;

        private readonly IChatContext _chatContext;

        private readonly IChatUserRepository _chatUserRepository;

        public ChatsController(
            IHttpContextAccessor httpContextAccessor,
            IHubContext<HubChatClient, IHubChatClient> hubChatContext,
            IChatContext chatContext,
            IChatUserRepository chatUserRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _hubChatContext = hubChatContext;
            _chatContext = chatContext;
            _chatUserRepository = chatUserRepository;
        }

        [HttpGet("who-online")]
        [Authorize]
        public IActionResult GetAllOnlineUser()
        {
            return Ok(_chatContext.GetOnlineUsers());
        }

        [HttpGet("all-users")]
        [Authorize]
        [ProducesResponseType(typeof(List<OnlineUser>), 200)]
        public async Task<IActionResult> GetAllChatUsers()
        {
            var foundChatUsers = await _chatUserRepository.GetAllAsync(a => a.Deactivate == false);
            var onlineUsers = new List<OnlineUser>();
            if (foundChatUsers != null)
            {
                foreach (var chatUser in foundChatUsers)
                {
                    var onlineUser = new OnlineUser
                    {
                        UserName = chatUser.UserName,
                        FullName = chatUser.FullName,
                        Avatar = chatUser.Avatar,
                        IsOnline = false
                    };
                    onlineUser.Load();
                    onlineUsers.Add(onlineUser);
                }
            }

            return Ok(onlineUsers);
        }

        [HttpPost("online")]
        [Authorize]
        public async Task<IActionResult> Online()
        {
            var jwtSecurityToken = _httpContextAccessor.HttpContext.Request.GetJwtToken();
            var fullName = jwtSecurityToken.GetFullName();
            var avatar = jwtSecurityToken.GetAvatar();
            var onlineUser = new OnlineUser
            {
                UserName = _httpContextAccessor.HttpContext.User.Identity.Name,
                FullName = fullName,
                Avatar = avatar,
                IsOnline = true
            };

            onlineUser.Load();
            // Persist Online user
            await _chatUserRepository.UpsertAsync(new Chat.Entities.ChatUser
            {
                UserName = onlineUser.UserName,
                Avatar = onlineUser.Avatar,
                FullName = onlineUser.FullName
            });
            await _chatContext.TakeOnlineAsync(onlineUser);
            // Boardcast all online users
            await _hubChatContext.Clients.All.Online(onlineUser);
            return Ok(onlineUser);
        }
    }
}
