using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Monitors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client.Workers
{
    public class MonitorHealthCheckBackgroundService : BackgroundService
    {
        private readonly IOptionsMonitor<ServiceOptions> _options;

        private readonly IMonitoHeartBeatChannel _channel;

        private readonly ILogger<MonitorHealthCheckBackgroundService> _logger;

        private MonitorService.MonitorServiceClient _client;

        public MonitorHealthCheckBackgroundService(
            IOptionsMonitor<ServiceOptions> options,
            IMonitoHeartBeatChannel channel,
            ILogger<MonitorHealthCheckBackgroundService> logger,
            MonitorService.MonitorServiceClient client)
        {
            _options = options;
            _channel = channel;
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting worker for sending health beat to Saturn at endpoint {saturnEndpoint} ", _options.CurrentValue.SaturnEndpoint);
            try
            {
                using var call = _client.Push(cancellationToken: stoppingToken);
                await foreach (var healthCheckRequest in _channel.GetChannel().Reader.ReadAllAsync(stoppingToken))
                {
                    await call.RequestStream.WriteAsync(healthCheckRequest);
                }
                await call.RequestStream.CompleteAsync();
                var serverResponse = await call.ResponseAsync;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There are some problems when trying to send HealthCheck");
            }
        }
    }
}
