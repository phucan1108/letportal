using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Microservices.Client.Options
{
    public class SaturnClientOptions
    {

        /// <summary>
        /// Enable to push HealthCheck to Saturn. Default: false
        /// Require: 
        /// MonitorOptions
        /// </summary>
        public bool EnableServiceMonitor { get; set; } = false;

        /// <summary>
        /// Enable to store log by Serilog. Default: false
        /// Require:
        /// LoggerOptions
        /// SerilogOptions
        /// </summary>
        public bool EnableSerilog { get; set; } = false;

        /// <summary>
        /// Enable to use built-in Cors of LetPortal. Default: false
        /// Require:
        /// CorsPortalOptions
        /// </summary>
        public bool EnableBuiltInCors { get; set; } = false;
    }
}
