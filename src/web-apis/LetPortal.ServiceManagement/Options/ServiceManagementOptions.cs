using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.ServiceManagement.Options
{
    public class ServiceManagementOptions
    {
        public int DurationLost { get; set; } = 120;

        public int DurationShutdown { get; set; } = 120;

        public int IntervalTime { get; set; } = 300;        
    }
}
