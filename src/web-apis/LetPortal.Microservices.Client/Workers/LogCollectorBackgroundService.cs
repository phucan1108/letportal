using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.LogCollector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client.Workers
{
    public class LogCollectorBackgroundService : BackgroundService
    {
        private readonly IOptionsMonitor<ServiceOptions> _options;

        private readonly ILogCollectorChannel _channel;

        private readonly ILogger<LogCollectorBackgroundService> _logger;

        private readonly LogCollectorService.LogCollectorServiceClient _client;

        public LogCollectorBackgroundService(
            IOptionsMonitor<ServiceOptions> options,
            ILogger<LogCollectorBackgroundService> logger,
            ILogCollectorChannel channel,
            LogCollectorService.LogCollectorServiceClient client)
        {
            _options = options;
            _channel = channel;
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting worker for sending log to Saturn at endpoint {saturnEndpoint}", _options.CurrentValue.SaturnEndpoint);

            try
            {
                using var call = _client.Push(cancellationToken: stoppingToken);
                await foreach (var log in _channel.GetChannel().Reader.ReadAllAsync(stoppingToken))
                {
                    await call.RequestStream.WriteAsync(log);
                }
                await call.RequestStream.CompleteAsync();
                var serverResponse = await call.ResponseAsync;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There are some problems when trying to stream log collector");
            }
        }
    }
}
