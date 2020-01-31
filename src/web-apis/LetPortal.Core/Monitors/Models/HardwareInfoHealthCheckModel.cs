namespace LetPortal.Core.Monitors.Models
{
    public class HardwareInfoHealthCheckModel
    {
        public double CpuUsage { get; set; }

        public long MemoryUsed { get; set; }

        public bool IsCpuBottleneck { get; set; }

        public bool IsMemoryThreshold { get; set; }
    }
}
