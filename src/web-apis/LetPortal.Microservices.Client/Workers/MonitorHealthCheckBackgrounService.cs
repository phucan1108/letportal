using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Monitors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client.Workers
{
    public class MonitorHealthCheckBackgrounService : BackgroundService
    {
        private readonly IOptionsMonitor<ServiceOptions> _options;

        private readonly IMonitoHeartBeatChannel _channel;

        private readonly ILogger<MonitorHealthCheckBackgrounService> _logger;

        private MonitorService.MonitorServiceClient _client;

        public MonitorHealthCheckBackgrounService(
            IOptionsMonitor<ServiceOptions> options,
            IMonitoHeartBeatChannel channel,
            ILogger<MonitorHealthCheckBackgrounService> logger,
            MonitorService.MonitorServiceClient client)
        {
            _options = options;
            _channel = channel;
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting worker for sending health beat to Saturn at endpoint " + _options.CurrentValue.SaturnEndpoint);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    while (await _channel.GetChannel().Reader.WaitToReadAsync())
                    {
                        using var call = _client.Push();
                        if(_channel.GetChannel().Reader.TryRead(out HealthCheckRequest healthCheckRequest))
                        {
                            await call.RequestStream.WriteAsync(healthCheckRequest);
                        }
                        await call.RequestStream.CompleteAsync();
                        var serverResponse = await call.ResponseAsync;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError("There are some problems when trying to send HealthCheck", ex.ToString());
                }                
            }
        }
    }
}
