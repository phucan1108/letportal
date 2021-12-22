using LetPortal.Core.Utils;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Extensions;
using LetPortal.Notification.Models;
using LetPortal.Notification.Models.RealTimes;
using LetPortal.Notification.Repositories.Channels;
using LetPortal.Notification.Repositories.MessageGroups;
using LetPortal.Notification.Repositories.Messages;
using LetPortal.Notification.Repositories.NotificationBoxMessages;
using LetPortal.Notification.Repositories.Subcribers;

namespace LetPortal.Notification.Services
{
    public class SubcriberService : ISubcriberService
    {
        private readonly IMessageRepository _messageRepository;

        private readonly IMessageGroupRepository _messageGroupRepository;

        private readonly INotificationBoxMessageRepository _notificationBoxMessageRepository;

        private readonly ISubscriberRepository _subscriberRepository;

        private readonly IChannelRepository _channelRepository;

        public SubcriberService(
            IMessageRepository messageRepository,
            IMessageGroupRepository messageGroupRepository,
            INotificationBoxMessageRepository notificationBoxMessageRepository,
            ISubscriberRepository subscriberRepository,
            IChannelRepository channelRepository
            )
        {
            _messageRepository = messageRepository;
            _messageGroupRepository = messageGroupRepository;
            _notificationBoxMessageRepository = notificationBoxMessageRepository;
            _subscriberRepository = subscriberRepository;
            _channelRepository = channelRepository;
        }


        public async Task<OnlineSubcriber> Subcribe(string userId, string userName, string[] roles)
        {
            // In case user didn't login yet, we should get in database
            var foundSubcriber = await _subscriberRepository.FindAsync(a => a.UserId == userId);

            if (foundSubcriber == null)
            {
                // It is first login time, create subcriber
                var newSubcriber = new Subcriber
                {
                    Id = DataUtil.GenerateUniqueId(),
                    Active = true,
                    Roles = roles,
                    UserId = userId,
                    UserName = userName,
                    LastClickedBoxTs = DateTime.UtcNow.Ticks
                };

                await _subscriberRepository.AddAsync(newSubcriber);

                return new OnlineSubcriber
                {
                    SubcriberId = newSubcriber.Id,
                    UserId = userId,
                    Roles = roles,
                    UserName = userName
                };
            }
            else
            {
                var newOnlineSubcriber = new OnlineSubcriber
                {
                    UserId = foundSubcriber.UserId,
                    Roles = roles,
                    SubcriberId = foundSubcriber.Id,
                    UserName = userName,
                    LastClickedTs = foundSubcriber.LastClickedBoxTs
                };

                // Get all message groups and take latest notification box
                var allMessageGroups = await 
                    _messageGroupRepository.GetAllAsync(a => a.SubcriberId == foundSubcriber.Id);
                foreach(var messageGroup in allMessageGroups)
                {
                    var onlineMessageGroup = new OnlineMessageGroup
                    {
                        Id = messageGroup.Id,
                        ChannelCode = messageGroup.ChannelCode,
                        Icon = messageGroup.Icon,
                        LastVisitedTs = messageGroup.LastVisitedTs,
                        Mute = messageGroup.Mute,
                        Name = messageGroup.Name,
                        SubcriberId = messageGroup.SubcriberId
                    };
                    var latestMessage = await _notificationBoxMessageRepository.TakeLastOfGroup(foundSubcriber.Id, messageGroup.Id);
                    if (latestMessage != null)
                    {
                        onlineMessageGroup.LastMessage = latestMessage.ToOnline();
                    }

                    var numberOfUnreadMessages = await _notificationBoxMessageRepository.CheckUnreadMessagesAsync(foundSubcriber.Id, messageGroup.Id, foundSubcriber.LastClickedBoxTs);
                    onlineMessageGroup.NumberOfUnreadMessages = numberOfUnreadMessages;
                    newOnlineSubcriber.Groups.Add(onlineMessageGroup);
                }

                newOnlineSubcriber.LastUnreadMessages = newOnlineSubcriber.Groups.Sum(a => a.NumberOfUnreadMessages);
                return newOnlineSubcriber;
            }
        }

        public async Task Receive(GlobalIncomingMessage globalMessage, Func<OnlineNotificationMessage, Task> postAction)
        {
            var allSubcribers = await _subscriberRepository.GetAllAsync(a => a.Active);
            foreach (var subcriber in allSubcribers)
            {
                await SendMessage(NotificationScope.Global, globalMessage, subcriber.Id, postAction);
            }
        }

        public async Task Receive(RoleIncomingMessage roleMessage, Func<OnlineNotificationMessage, Task> postAction)
        {
            var allSubcribersMatchedRoles = await _subscriberRepository.GetByRoles(roleMessage.Roles.ToArray());

            foreach (var subcriber in allSubcribersMatchedRoles)
            {
                await SendMessage(NotificationScope.Roles, roleMessage, subcriber.Id, postAction);
            }
        }

        public async Task Receive(IndividualIncomingMessage incomingMessage, Func<OnlineNotificationMessage, Task> postAction)
        {
            var subcriber = await _subscriberRepository.FindAsync(a => a.UserId == incomingMessage.Target);

            await SendMessage(NotificationScope.Individual, incomingMessage, subcriber.Id, postAction);
        }

        private async Task SendMessage(
            NotificationScope scope,
            IncomingMessage incomingMessage,
            string subcriberId,
            Func<OnlineNotificationMessage, Task> postAction)
        {
            var foundMessageGroup = await _messageGroupRepository.FindAsync(a => a.SubcriberId == subcriberId && a.ChannelCode == incomingMessage.Code);

            if (foundMessageGroup == null)
            {
                var channel = await _channelRepository.FindAsync(a => a.Code == incomingMessage.Code);

                foundMessageGroup = new MessageGroup
                {
                    Id = DataUtil.GenerateUniqueId(),
                    ChannelCode = incomingMessage.Code,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    Mute = false,
                    Name = channel.Name,
                    Icon = channel.Icon,
                    LastVisitedTs = DateTime.UtcNow.Ticks,
                    SubcriberId = subcriberId
                };

                await _messageGroupRepository.AddAsync(foundMessageGroup);
            }

            var message = incomingMessage.ToMessage(foundMessageGroup.Id);
            await _messageRepository.AddAsync(message);

            var notificationMessage = incomingMessage.ToNotificationMessage(subcriberId, foundMessageGroup.Id, message.Id);
            await _notificationBoxMessageRepository.AddAsync(notificationMessage);

            var realTimeMessage = new OnlineNotificationMessage
            {
                NotificationBoxId = notificationMessage.Id,
                MessageGroupId = foundMessageGroup.Id,
                MessageId = message.Id,
                ShortMessage = notificationMessage.ShortMessage,
                SubcriberId = subcriberId,
                Type = notificationMessage.Type,
                IsDirty = false,
                ReceivedDate = notificationMessage.ReceivedDate,
                ReceivedDateTs = notificationMessage.ReceivedDateTs,
                ClickedDate = notificationMessage.ClickedDate,
                MessageGroupName = foundMessageGroup.Name
            };

            if (postAction != null)
            {
                await postAction.Invoke(realTimeMessage);
            }

        }

        public async Task MarkRead(string subcriberId, string notificationId)
        {
            var notificationBoxMessage = await _notificationBoxMessageRepository.FindAsync(a => a.Id == notificationId);

            if (notificationBoxMessage != null)
            {
                notificationBoxMessage.IsDirty = true;
                notificationBoxMessage.ClickedDate = DateTime.UtcNow;

                await _notificationBoxMessageRepository.UpdateAsync(notificationId, notificationBoxMessage);
            }
        }

        public async Task ClickOnNotificationBox(string subcriberId, long clickedTicks)
        {
            await _subscriberRepository.UpdateLastClicked(subcriberId, clickedTicks);
        }
    }
}
