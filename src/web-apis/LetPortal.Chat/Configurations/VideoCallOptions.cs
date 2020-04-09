using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Configurations
{
    public class VideoCallOptions
    {
        /// <summary>
        /// Add many public STUN servers as we wanted
        /// Prefer the list in https://gist.github.com/mondain/b0ec1cf5f60ae726202e
        /// </summary>
        public List<RtcIceServer> IceServers { get; set; }
    }

    public class RtcIceServer
    {
        public string Urls { get; set; }
        public string Username { get; set; }
        public string Credential { get; set; }
    }
}
