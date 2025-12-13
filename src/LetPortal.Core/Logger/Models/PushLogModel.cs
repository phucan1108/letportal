using System;
using System.Collections.Generic;

namespace LetPortal.Core.Logger.Models
{
    public class PushLogModel
    {
        public string RegisteredServiceId { get; set; }

        public string ServiceName { get; set; }

        public string UserSessionId { get; set; }

        public string TraceId { get; set; }

        public List<string> StackTraces { get; set; }

        public string HttpRequestUrl { get; set; }

        public string HttpHeaders { get; set; }

        public string HttpRequestBody { get; set; }

        public int ResponseStatusCode { get; set; }

        public string ResponseBody { get; set; }

        public double CpuUsage { get; set; }

        public double MemoryUsed { get; set; }

        public DateTime BeginRequest { get; set; }

        public DateTime EndRequest { get; set; }

        public long ElapsedTime { get; set; }
    }
}
