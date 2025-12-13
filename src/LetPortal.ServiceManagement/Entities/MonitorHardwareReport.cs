using System;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "monitorhardwarereports")]
    [Table("monitorhardwarereports")]
    public class MonitorHardwareReport : Entity
    {
        public string ServiceId { get; set; }

        public double CpuUsage { get; set; }

        public long MemoryUsed { get; set; }

        public int MemoryUsedInMb { get; set; }

        public bool IsCpuBottleneck { get; set; }

        public bool IsMemoryThreshold { get; set; }

        public DateTime ReportedDate { get; set; }
    }
}
