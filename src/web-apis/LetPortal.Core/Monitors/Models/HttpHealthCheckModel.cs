using System;

namespace LetPortal.Core.Monitors.Models
{
    public class HttpHealthCheckModel
    {
        public DateTime MeansureDateTime { get; set; }

        public int TotalRequestsPerDay { get; set; }

        public double AvgDuration { get; set; }

        public int SuccessRequests { get; set; }

        public int FailedRequests { get; set; }
    }
}
