using System.Collections.Generic;
using LetPortal.Chat.Models;

namespace LetPortal.Chat
{
    public interface IVideoContext
    {
        void AddVideoRoom(VideoRoomModel roomModel);

        VideoRoomModel GetRoom(string roomId);

        bool IsExistedRoom(string participantOne, string participantTwo);

        bool IsUserInCall(string userName);

        void StopRoom(string roomId);

        List<VideoRoomModel> GetAllAvailableRooms(string connectionId);
    }
}
