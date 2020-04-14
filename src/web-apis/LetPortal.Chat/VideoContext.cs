using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetPortal.Chat.Models;

namespace LetPortal.Chat
{
    public class VideoContext : IVideoContext
    {
        public List<VideoRoomModel> VideoRooms { get; set; } = new List<VideoRoomModel>();        

        public void AddVideoRoom(VideoRoomModel roomModel)
        {
            VideoRooms.Add(roomModel);
        }

        public List<VideoRoomModel> GetAllAvailableRooms(string connectionId)
        {
            return VideoRooms.Where(a => a.Participants.Any(b => b.ConnectionId == connectionId) && !a.IsStoppped).ToList();
        }

        public VideoRoomModel GetRoom(string roomId)
        {
            return VideoRooms.First(a => a.Id == roomId);
        }

        public bool IsExistedRoom(string participantOne, string participantTwo)
        {
            return VideoRooms
                .Any(a => 
                    a.Participants.Any(b => b.Username == participantOne) 
                    && a.Participants.Any(c => c.Username == participantTwo));
        }

        public bool IsUserInCall(string userName)
        {
            return VideoRooms.Any(a => !a.IsStoppped && a.Participants.Any(b => b.Username == userName));
        }

        public void StopRoom(string roomId)
        {
            var foundRoom = VideoRooms.First(a => a.Id == roomId);
            if(foundRoom != null)
            {
                foundRoom.IsStoppped = true;
                foundRoom.DroppedDate = DateTime.UtcNow;
            }
        }
    }
}
