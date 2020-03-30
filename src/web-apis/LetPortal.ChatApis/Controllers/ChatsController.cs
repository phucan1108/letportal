using System.Threading.Tasks;
using LetPortal.Chat;
using LetPortal.Chat.Hubs;
using LetPortal.Chat.Models;
using LetPortal.Core.Https;
using LetPortal.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LetPortal.ChatApis.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHubContext<HubChatClient, IHubChatClient> _hubChatContext;

        private readonly IChatContext _chatContext;

        public ChatsController(
            IHttpContextAccessor httpContextAccessor,
            IHubContext<HubChatClient, IHubChatClient> hubChatContext,
            IChatContext chatContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _hubChatContext = hubChatContext;
            _chatContext = chatContext;
        }

        [HttpGet("who-online")]
        [Authorize]
        public IActionResult GetAllOnlineUser()
        {
            return Ok(_chatContext.GetOnlineUsers());
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
                Avatar = avatar
            };

            onlineUser.Load();
            await _chatContext.TakeOnlineAsync(onlineUser);
            // Boardcast all online users
            await _hubChatContext.Clients.All.Online(onlineUser);
            return Ok();
        }
         
        [HttpPost("offline")]
        [Authorize]
        public async Task<IActionResult> Offline()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            await _chatContext.TakeOfflineAsync(new OnlineUser
            {
                UserName = userName
            });
            // Boardcast all online users
            await _hubChatContext.Clients.All.Offline(userName);
            return Ok();
        }
    }
}
