using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LetPortal.Core.Monitors
{
    public class LetPortalMonitorHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IServiceContext _serviceContext;

        public LetPortalMonitorHealthCheckPublisher(
            IServiceContext serviceContext
            )
        {
            _serviceContext = serviceContext;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var healthcheck = report.Entries.GetValueOrDefault(Constants.LetPortalHealthCheck);
            var pulse = healthcheck.Data[Constants.LetPortalHealthCheckData] as PushHealthCheckModel;
            _serviceContext.PushHealthCheck(pulse);

            // Keep updating run state
            _serviceContext.Run(null);

            return Task.CompletedTask;
        }
    }
}
