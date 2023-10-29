using LetPortal.Core.Https;
using LetPortal.Core.Logger;
using LetPortal.Core.Notifications;
using LetPortal.Core.Security;
using LetPortal.Notification.Extensions;
using LetPortal.Notification.Hubs;
using LetPortal.Notification.Models.Apis;
using LetPortal.Notification.Models.RealTimes;
using LetPortal.Notification.Repositories.MessageGroups;
using LetPortal.Notification.Repositories.NotificationBoxMessages;
using LetPortal.Notification.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NSwag.Annotations;

namespace LetPortal.Notification.AppParts.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [OpenApiIgnore]
    public class NotificationsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;        

        private readonly IServiceLogger<NotificationsController> _logger;

        private readonly NotificationRealTimeContext _notificationRealTimeContext;

        private readonly ISubcriberService _subcriberService;

        private readonly IMessageGroupRepository _messageGroupRepository;

        private readonly INotificationBoxMessageRepository _notificationBoxMessageRepository;

        public NotificationsController(
                IHttpContextAccessor httpContextAccessor,
                IServiceLogger<NotificationsController> logger,
                ISubcriberService subcriberService,
                NotificationRealTimeContext notificationRealTimeContext,
                IMessageGroupRepository messageGroupRepository,
                INotificationBoxMessageRepository notificationBoxMessageRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _subcriberService = subcriberService;
            _notificationRealTimeContext = notificationRealTimeContext;
            _messageGroupRepository = messageGroupRepository;
            _notificationBoxMessageRepository = notificationBoxMessageRepository;
        }

        [HttpPost("subcribe")]
        [Authorize]
        [ProducesResponseType(typeof(OnlineSubcriber), 200)]
        public async Task<IActionResult> Subcribe()
        {
            var jwtSecurityToken = _httpContextAccessor.HttpContext.Request.GetJwtToken();
            var userId = jwtSecurityToken.Subject;

            var onlineSubcriber = await _subcriberService.Subcribe(userId, jwtSecurityToken.GetUserName(), jwtSecurityToken.GetRoles());

            if(_notificationRealTimeContext.OnlineSubcribers.Any(a => a.SubcriberId == onlineSubcriber.SubcriberId))
            {
                _notificationRealTimeContext.RemoveSubcriber(onlineSubcriber);
                _notificationRealTimeContext.AddSubcriber(onlineSubcriber);
            }
            else
            {
                _notificationRealTimeContext.AddSubcriber(onlineSubcriber);
            }

            return Ok(onlineSubcriber);
        }
        
        [HttpGet("message-groups/{subcriberId}")]
        [ProducesResponseType(typeof(List<OnlineMessageGroup>), 200)]
        public async Task<IActionResult> GetMessageGroups(string subcriberId)
        {
            var messageGroups = await _messageGroupRepository.GetAllAsync(a => a.SubcriberId == subcriberId);
            if (messageGroups.Any())
            {
                return Ok(messageGroups.Select(a => a.ToOnline()));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("messages/fetch")]
        [ProducesResponseType(typeof(List<OnlineNotificationMessage>), 200)]
        public async Task<IActionResult> FetchMessages(
            FetchedNotificationMessageRequest request)
        {
            var allMessages = await _notificationBoxMessageRepository
                .TakeLastAsync(
                    request.SubcriberId,
                    request.MessageGroupId,
                    request.LastFectchedTs, 
                    20, 
                    request.SelectedTypes);
            if (allMessages.Any())
            {
                return Ok(allMessages.Select(a => a.ToOnline()));
            }
            else
            {
                return Ok();
            }
        }
    }
}
