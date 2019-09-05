using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "monitorcounters")]
    public class MonitorCounter : Entity
    {
        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public HttpCounter HttpCounter { get; set; }

        public HardwareCounter HardwareCounter { get; set; }

        public DateTime BeatDate { get; set; }
    }

    public class HardwareCounter
    {
        public double CpuUsage { get; set; }

        public long MemoryUsed { get; set; }

        public bool IsCpuBottleneck { get; set; }

        public bool IsMemoryThreshold { get; set; }
    }

    public class HttpCounter
    {
        public DateTime MeansureDateTime { get; set; }

        public int TotalRequestsPerDay { get; set; }

        public double AvgDuration { get; set; }

        public int SuccessRequests { get; set; }

        public int FailedRequests { get; set; }
    }
}
