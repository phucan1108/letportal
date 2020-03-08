using System;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "monitorhttpreports")]
    [Table("monitorhttpreports")]
    public class MonitorHttpReport : Entity
    {
        public string ServiceId { get; set; }

        public int SuccessRequests { get; set; }

        public int FailRequests { get; set; }

        public int TotalRequests { get; set; }

        public double AvgDuration { get; set; }

        public DateTime ReportedDate { get; set; }
    }
}
