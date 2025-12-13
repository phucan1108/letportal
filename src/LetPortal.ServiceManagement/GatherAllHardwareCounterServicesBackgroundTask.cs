using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.ServiceManagement.Options;
using LetPortal.ServiceManagement.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.ServiceManagement
{
    public class GatherAllHardwareCounterServicesBackgroundTask : BackgroundService
    {
        private readonly ILogger<GatherAllHardwareCounterServicesBackgroundTask> _logger;

        private readonly IOptionsMonitor<ServiceManagementOptions> _options;

        private readonly IServiceProvider _services;

        private int numberRun = 0;

        public GatherAllHardwareCounterServicesBackgroundTask(
            IServiceProvider services,
            ILogger<GatherAllHardwareCounterServicesBackgroundTask> logger,
            IOptionsMonitor<ServiceManagementOptions> options)
        {
            _services = services;
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Setting up Gather Services Report cron task with options {options}", _options);
            while (!stoppingToken.IsCancellationRequested)
            {
                var monitorService = _services.GetService<IMonitorServiceReportProvider>();
                var serviceManagement = _services.GetService<IServiceManagementProvider>();
                try
                {
                    var allRunningServiceIds = await serviceManagement.GetAllRunningServices();
                    await monitorService.CollectAndReportHardware(allRunningServiceIds, _options.CurrentValue.DurationMonitorReport);
                    await monitorService.CollectAndReportHttp(allRunningServiceIds, _options.CurrentValue.DurationMonitorReport);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during running Services Report Cron Task");
                }

                await Task.Delay(_options.CurrentValue.IntervalMonitorReport * 1000, stoppingToken);

                numberRun++;

                _logger.LogInformation("Gather Services Report in {numberRun}", numberRun);
            }
        }
    }
}
