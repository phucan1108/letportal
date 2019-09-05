namespace LetPortal.Core.Services.Models
{
    public class RegisterServiceModel
    {
        public string ServiceName { get; set; }

        public string Version { get; set; }

        public string IpAddress { get; set; }

        public ServiceHardwareInfo ServiceHardwareInfo { get; set; }

        public bool LoggerNotifyEnable { get; set; }

        public bool HealthCheckNotifyEnable { get; set; }
    }

    public class ServiceHardwareInfo
    {
        public int ProcessorCores { get; set; }

        public string Os { get; set; }

        public string MachineName { get; set; }

        public string Directory { get; set; }
    }
}
