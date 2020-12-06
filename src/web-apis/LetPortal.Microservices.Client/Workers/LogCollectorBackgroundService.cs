using System;
using System.Collections.Generic;
using System.Text;
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
            _logger.LogInformation("Starting worker for sending log to Saturn at endpoint " + _options.CurrentValue.SaturnEndpoint);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting to read log from channel");
                try
                {
                    while (await _channel.GetChannel().Reader.WaitToReadAsync())
                    {
                        using var call = _client.Push();
                        if(_channel.GetChannel().Reader.TryRead(out LogCollectorRequest log))
                        {
                            await call.RequestStream.WriteAsync(log);
                        }
                        await call.RequestStream.CompleteAsync();
                        var serverResponse = await call.ResponseAsync;
                    }
                }
                 catch(Exception ex)
                {
                    _logger.LogError("There are some problems when trying to stream log collector", ex.ToString());
                }  
            }
        }
    }
}
