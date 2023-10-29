using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Notification.Options
{
    public class NotificationOptions
    {
        /// <summary>
        /// Allow drivers for queue: MongoDb, RabbitMq. Default: RabbitMq
        /// </summary>
        public string Driver { get; set; } = "RabbitMq";

        public string ConnectionString { get; set; }

        public string QueueName { get; set; } = "SaturnNotificationQueue";

        public int DelayPullMessageInMs { get; set; } 

    }
}
