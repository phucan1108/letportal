using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Microservices.Client.Models
{
    public class NotificationMessage
    {
        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string Sender { get; set; }

        public string ChannelCode { get; set; }

        public string Message { get; set; }

        public string IndividualUsername { get; set; }
    }
}
