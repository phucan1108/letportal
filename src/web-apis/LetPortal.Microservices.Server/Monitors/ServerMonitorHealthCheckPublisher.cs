using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Microservices.Monitors;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LetPortal.Microservices.Server.Monitors
{
    public class ServerMonitorHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IServiceContext _serviceContext;

        public ServerMonitorHealthCheckPublisher(
            IServiceContext serviceContext
            )
        {
            _serviceContext = serviceContext;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var healthcheck = report.Entries.GetValueOrDefault(Constants.SERVER_MONITOR_HEALTHCHECK);
            if(healthcheck.Data.ContainsKey(Constants.SERVER_MONITOR_HEALTHCHECK_DATA))
            {
                var pulse = healthcheck.Data[Constants.SERVER_MONITOR_HEALTHCHECK_DATA] as HealthCheckRequest;
                _serviceContext.PushHealthCheck(pulse);
            }
            

            // Keep updating run state
            _serviceContext.Run(null);

            return Task.CompletedTask;
        }
    }
}
