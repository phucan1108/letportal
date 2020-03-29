using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Configurations
{
    public class ChatOptions
    {
        public List<string> AllowedHosts { get; set; }

        /// <summary>
        /// Due to memory capacity, we need to restrict maximum online users
        /// </summary>
        public int MaximumConcurrentUsers { get; set; } = 50;

        /// <summary>
        /// Due to memory capacity, we need to restrict chat rooms per user
        /// </summary>
        public int MaximumChatRoomsPerUsers { get; set; } = 10;

        /// <summary>
        /// This option will be used to indicate when creating new Chat Session
        /// </summary>
        public int ThresholdNumberOfMessages { get; set; } = 200;
    }
}
