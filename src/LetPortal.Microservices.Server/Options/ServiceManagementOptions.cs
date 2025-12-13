namespace LetPortal.Microservices.Server.Options
{
    public class ServiceManagementOptions
    {
        public int DurationLost { get; set; } = 120;

        public int DurationShutdown { get; set; } = 120;

        public int DurationMonitorReport { get; set; } = 300;

        public int IntervalLost { get; set; } = 300;

        public int IntervalShutdown { get; set; } = 300;

        public int IntervalMonitorReport { get; set; } = 300;
    }
}
