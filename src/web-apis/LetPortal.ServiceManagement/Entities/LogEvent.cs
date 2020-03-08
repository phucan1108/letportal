using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "logevents")]
    [Table("logevents")]
    public class LogEvent : Entity
    {
        public string TraceId { get; set; }

        public string Source { get; set; }

        public string SourceId { get; set; }

        public IEnumerable<string> StackTrace { get; set; }

        public string HttpRequestUrl { get; set; }

        public string HttpRequestHeaders { get; set; }

        public string HttRequestBody { get; set; }

        public string HttpResponseBody { get; set; }

        public int HttpResponseStatusCode { get; set; }

        public long ElapsedTime { get; set; }

        public DateTime BeginRequest { get; set; }

        public DateTime EndRequest { get; set; }

        public double CpuUsage { get; set; }

        public double MemoryUsed { get; set; }
    }
}
