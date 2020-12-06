using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Microservices.Server.Options;
using LetPortal.Microservices.Server.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Server.Workers
{
    public class CheckingLostServicesBackgroundTask : BackgroundService
    {
        private readonly ILogger<CheckingLostServicesBackgroundTask> _logger;

        private readonly IOptionsMonitor<ServiceManagementOptions> _options;

        private readonly IServiceProvider _services;

        public CheckingLostServicesBackgroundTask(
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
                try
                {
                    // Create Cleaning Lost
                    var serviceManagement = _services.GetService<IServiceManagementProvider>();

                    await serviceManagement.CheckAndUpdateAllLostServices(_options.CurrentValue.DurationLost);
                }
                catch(Exception ex)
                {
                    _logger.LogError("There are some problem when trying to UpdateAllLostServices", ex.ToString());
                }

                await Task.Delay(_options.CurrentValue.IntervalLost * 1000, stoppingToken);
            }
        }
    }
}
