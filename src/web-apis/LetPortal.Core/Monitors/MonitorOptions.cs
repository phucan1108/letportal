namespace LetPortal.Core.Monitors
{
    public class MonitorOptions
    {
        public bool Enable { get; set; }

        public MonitorNotifyOptions NotifyOptions { get; set; }

        /// <summary>
        /// It helps to reset monitor counter at 0, based on Timezone. Default: "SE Asia Standard Time" UTC+07
        /// </summary>
        public string ResetMonitorTimezone { get; set; } = "SE Asia Standard Time";

        /// <summary>
        /// Allow open '/health' to be called
        /// Note: it will use the same port with Kestrel
        /// </summary>
        public bool AllowHttpEndpoint { get; set; } = false;

        /// <summary>
        /// Allow send healthcheck data to Service Management
        /// </summary>
        public bool AllowNotify { get; set; } = true;

        /// <summary>
        /// Value defines a bottleneck of cpu, in %. Default: 70
        /// </summary>
        public double BottleneckCpu { get; set; } = 70;

        /// <summary>
        /// Value defines a threshold memory, in Kb. Default: 819200Kb
        /// </summary>
        public long ThresholdMemory { get; set; } = 819200;

        public bool CheckHttpRequest { get; set; } = true;
        public bool CheckCpuBottleneck { get; set; } = true;
        public bool CheckMemoryThreshold { get; set; } = true;

        /// <summary>
        /// Check Database connection in DatabaseOptions, if not found, no data is sent
        /// </summary>
        public bool CheckDatabaseOption { get; set; } = false;
    }

    public class MonitorNotifyOptions
    {
        public bool Enable { get; set; }

        public int Delay { get; set; } = 5;
    }
}
