using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetPortal.Microservices.Server.Entities
{
    [Table("servicehardwareinfos")]
    public class ServiceHardwareInfo
    {
        public string Id { get; set; }

        public string ServiceId { get; set; }

        public int ProcessorCores { get; set; }

        public string Os { get; set; }

        public string MachineName { get; set; }

        public string Directory { get; set; }

        public long AllocationMemory { get; set; }
    }
}
