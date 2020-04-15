using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Configurations;
using LetPortal.Chat.Exceptions;
using LetPortal.Chat.Models;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LetPortal.Chat.Hubs
{
    [Authorize]
    public class HubVideoClient : Hub<IHubVideoClient>
    {
        private readonly IVideoContext _videoContext;

        private readonly IOptionsMonitor<VideoCallOptions> _options;

        public HubVideoClient(IVideoContext videoContext, IOptionsMonitor<VideoCallOptions> options)
        {
            _videoContext = videoContext;
            _options = options;
        }

        public HubVideoClient(IVideoContext videoContext)
        {
            _videoContext = videoContext;
        }

        public async Task SignalingUser(Models.OnlineUser invitee)
        {
            var requesterInCall = _videoContext.IsUserInCall(Context.UserIdentifier);
            var inviteeInCall = _videoContext.IsUserInCall(invitee.UserName);
            if (!requesterInCall && !inviteeInCall)
            {
                await Clients.User(invitee.UserName).SignalingIncomingCall(new Models.ParticipantVideoModel
                {
                    ConnectionId = Context.ConnectionId,
                    Username = Context.UserIdentifier
                });
            }
            else if(requesterInCall)
            {
                await Clients.Client(Context.ConnectionId).ReceivedException(new VideoErrorModel
                {
                    Error = ErrorCodes.RequestUserIsInCall
                });
            }
            else if (inviteeInCall)
            {
                await Clients.Client(Context.ConnectionId).ReceivedException(new VideoErrorModel
                {
                    Error = ErrorCodes.UserIsInCall
                });
            }
        }

        public async Task CancelCall(Models.OnlineUser invitee)
        {
            await Clients.User(invitee.UserName).CancelVideoCall(Context.UserIdentifier);
        }

        public async Task DenyCall(ParticipantVideoModel caller)
        {
            await Clients.Client(caller.ConnectionId).DeniedRequestCall(Context.UserIdentifier);
        }

        public async Task AnsweringVideoCall(Models.ParticipantVideoModel caller)
        {
            var answeredUser = new Models.ParticipantVideoModel
            {
                Username = Context.UserIdentifier,
                ConnectionId = Context.ConnectionId
            };

            var handshakeRoom = new Models.VideoRoomModel
            {
                Id = DataUtil.GenerateUniqueId(),
                HandshakeDate = DateTime.UtcNow,
                Participants = new List<Models.ParticipantVideoModel>
                    {
                        caller,
                        answeredUser
                    }
            };
            _videoContext.AddVideoRoom(handshakeRoom);

            await Clients
                .Clients(handshakeRoom.Participants.Select(a => a.ConnectionId).ToArray())
                .HandshakedVideoCall(handshakeRoom);

            // Send ICE Server for letting them create P2P network
            await Clients.Client(caller.ConnectionId).ReceivedIceServer(_options.CurrentValue.IceServers);
            await Clients.Client(answeredUser.ConnectionId).ReceivedIceServer(_options.CurrentValue.IceServers);
        }

        public async Task SendRtcSignal(VideoRtcSignalModel model)
        {
            var room = _videoContext.GetRoom(model.RoomId);
            if (room != null && !room.IsStoppped)
            {
                // Ensure there are no mistaken between two connections
                if (room.Participants.Any(a => a.ConnectionId == model.ConnectionId)
                    && room.Participants.Any(a => a.ConnectionId == Context.ConnectionId))
                {
                    await Clients.Client(model.ConnectionId).ReceivedRtcSignal(new VideoRtcSignalModel
                    {
                        ConnectionId = Context.ConnectionId,
                        RoomId = model.RoomId,
                        SignalMessage = model.SignalMessage
                    });
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).ReceivedException(new VideoErrorModel
                    {
                        Error = ErrorCodes.WrongVideoRoom
                    });
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).ReceivedException(new VideoErrorModel
                {
                    Error = ErrorCodes.VideoCallHasBeenEnd
                });
            }
        }

        public async Task DropCall(string videoRoomId)
        {
            var room = _videoContext.GetRoom(videoRoomId);
            if(room != null && !room.IsStoppped)
            {
                _videoContext.StopRoom(videoRoomId);
                var invitee = room.Participants.First(a => a.ConnectionId != Context.ConnectionId);
                await Clients.Client(invitee.ConnectionId).ReceivedDroppedCall(videoRoomId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var allRelatedRooms = _videoContext.GetAllAvailableRooms(Context.ConnectionId);

            if(allRelatedRooms != null)
            {
                foreach(var room in allRelatedRooms)
                {
                    _videoContext.StopRoom(room.Id);
                    await Clients.Client(room.Participants.First(a => a.ConnectionId != Context.ConnectionId).ConnectionId).ReceivedDroppedCall(room.Id);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
