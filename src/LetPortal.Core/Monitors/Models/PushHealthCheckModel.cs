using System;

namespace LetPortal.Core.Monitors.Models
{
    public class PushHealthCheckModel
    {
        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public bool Healthy { get; set; }

        public bool DatabaseHealthy { get; set; }

        public HttpHealthCheckModel HttpHealthCheck { get; set; } = new HttpHealthCheckModel();

        public HardwareInfoHealthCheckModel HardwareInfoHealthCheck { get; set; } = new HardwareInfoHealthCheckModel();

        public string ErrorStack { get; set; }

        public DateTime BeatDate { get; set; }
    }
}
