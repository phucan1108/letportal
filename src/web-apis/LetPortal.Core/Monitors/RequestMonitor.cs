using System;

namespace LetPortal.Core.Monitors
{
    public class RequestMonitor
    {
        public double CpuUsage { get; set; }

        public long MemoryUsed { get; set; }

        public int StatusCode { get; set; }

        public DateTime BeginDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public long ElapsedTime { get; set; }
    }
}
