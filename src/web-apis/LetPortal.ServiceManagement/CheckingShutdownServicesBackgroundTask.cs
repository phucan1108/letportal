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
    public class CheckingShutdownServicesBackgroundTask : BackgroundService
    {
        private readonly ILogger<CheckingLostServicesBackgroundTask> _logger;

        private readonly IOptionsMonitor<ServiceManagementOptions> _options;

        private readonly IServiceProvider _services;

        private int numberRun = 0;

        public CheckingShutdownServicesBackgroundTask(
            IServiceProvider services,
            ILogger<CheckingLostServicesBackgroundTask> logger,
            IOptionsMonitor<ServiceManagementOptions> options)
        {
            _services = services;
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Setting up lost cron task and shutdown cron task with options {options}", _options);
            while (!stoppingToken.IsCancellationRequested)
            {
                // Create Cleaning Lost
                var serviceManagement = _services.GetService<IServiceManagementProvider>();

                await serviceManagement.CheckAndShutdownAllLostServices(_options.CurrentValue.DurationShutdown);

                await Task.Delay(_options.CurrentValue.IntervalShutdown * 1000, stoppingToken);

                numberRun++;

                _logger.LogInformation("Cleaning Shutdown Services in {numberRun}", numberRun);
            }
        }
    }
}
