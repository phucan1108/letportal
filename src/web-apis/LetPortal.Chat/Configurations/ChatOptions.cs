using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Configurations
{
    public class ChatOptions
    {
        /// <summary>
        /// Maximum chat session will be stored in memory
        /// Due to memory capacity, we need to restrict maximum sessions per room
        /// For example: One in-memory message takes around maximum 6Kb 
        /// One session has 50 messages so it takes 6*50 = 300Kb
        /// One room has up to 5 sessions so it takes 300*5 = 1.5Mb
        /// One user has up to 5 rooms so it takes 1.5*5 = 7.5Mb
        /// So 20 users can take possibly around 150Mb memory
        /// Therefore, retrict maximum number of sessions is a good way to prevent overflow
        /// </summary>
        public int MaximumSessionsPerChatRoom { get; set; } = 5;

        /// <summary>
        /// This option will be used to indicate when creating new Chat Session
        /// </summary>
        public int ThresholdNumberOfMessages { get; set; } = 50;
    }
}
