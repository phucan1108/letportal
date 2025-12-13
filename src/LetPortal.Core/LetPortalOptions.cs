namespace LetPortal.Core
{
    public class LetPortalOptions
    {
        /// <summary>
        /// Enable to connect with database. Default: true
        /// Require:
        /// DatabaseOptions
        /// </summary>
        public bool EnableDatabaseConnection { get; set; } = true;

        /// <summary>
        /// Enable to connect with Service Management. Default: false
        /// Require: ServiceOptions
        /// </summary>
        public bool EnableMicroservices { get; set; } = false;

        /// <summary>
        /// Enable to push HealthCheck to Service Management. Default: false
        /// Require: 
        /// MonitorOptions
        /// EnableMicroservices = true
        /// </summary>
        public bool EnableServiceMonitor { get; set; } = false;

        /// <summary>
        /// Enable to store log by Serilog. Default: false
        /// Require:
        /// LoggerOptions
        /// SerilogOptions
        /// EnableMicroservices = true
        /// </summary>
        public bool EnableSerilog { get; set; } = false;

    }
}
