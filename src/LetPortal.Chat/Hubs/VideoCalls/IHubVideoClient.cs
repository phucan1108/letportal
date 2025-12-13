using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Configurations;
using LetPortal.Chat.Models;

namespace LetPortal.Chat.Hubs
{
    public interface IHubVideoClient
    {
        Task SignalingIncomingCall(ParticipantVideoModel caller);

        Task CancelVideoCall(string caller);

        Task HandshakedVideoCall(VideoRoomModel videoRoom);

        Task DeniedRequestCall(string receiver);

        Task ReceivedDroppedCall(string roomId);

        Task ReceivedException(VideoErrorModel error);

        Task ReceivedIceServer(List<RtcIceServer> iceServers);

        Task ReceivedRtcSignal(VideoRtcSignalModel message);
    }
}
