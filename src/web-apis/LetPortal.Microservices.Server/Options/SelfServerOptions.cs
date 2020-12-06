namespace LetPortal.Microservices.Server.Options
{
    public class SelfServerOptions
    {
        public string ServerName { get; set; } = "Saturn";

        /// <summary>
        /// Because Server must be injected DatabaseOptions so we need to ensure no conflict
        /// Require:
        /// DatabaseOptions
        /// </summary>
        public bool EnableDatabaseOptions { get; set; } = false;

        /// <summary>
        /// Allow self-server to monitor itself
        /// Require:
        /// MonitorOptions
        /// </summary>
        public bool EnableServiceMonitor { get; set; } = false;

        public bool EnableSerilog { get; set; } = false;

        public bool EnableBuiltInCors { get; set; } = false;
    }
}
