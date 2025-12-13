using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Notification.Models
{
    public class RoleIncomingMessage : IncomingMessage
    {
        public IList<string> Roles { get; set; }
    }
}
