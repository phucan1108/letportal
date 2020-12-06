using System;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Client.Exceptions;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Monitors;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client
{
    class ClientServiceContext : IServiceContext
    {
        private static string serviceId;

        private readonly IOptionsMonitor<ServiceOptions> _serviceOptions;

        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;

        private readonly IOptionsMonitor<MonitorOptions> _monitorOptions;

        private readonly ILogCollectorChannel _logCollectorChannel;

        private readonly IMonitoHeartBeatChannel _monitoHeartBeatChannel;

        private readonly MonitorService.MonitorServiceClient _monitorServiceClient;

        public ClientServiceContext(
            IOptionsMonitor<ServiceOptions> serviceOptions,
            IOptionsMonitor<MonitorOptions> monitorOptions,
            IOptionsMonitor<LoggerOptions> loggerOptions,
            ILogCollectorChannel logCollectorChannel,
            IMonitoHeartBeatChannel monitoHeartBeatChannel,
            MonitorService.MonitorServiceClient monitorServiceClient)
        {
            _serviceOptions = serviceOptions;
            _monitorOptions = monitorOptions;
            _loggerOptions = loggerOptions;
            _logCollectorChannel = logCollectorChannel;
            _monitoHeartBeatChannel = monitoHeartBeatChannel;
            _monitorServiceClient = monitorServiceClient;
        }

        public void Start(Action postAction)
        {
            try
            {
                var registerServiceRequest = new RegisterServiceRequest
                {
                    ServiceName = _serviceOptions.CurrentValue.Name,
                    Version = _serviceOptions.CurrentValue.Version,
                    LoggerNotifyEnable = _loggerOptions.CurrentValue.NotifyOptions.Enable,
                    HealthCheckNotifyEnable = _monitorOptions.CurrentValue.Enable,
                    ServiceHardwareInfo = new ServiceHardwareInfo
                    {
                        Directory = Environment.CurrentDirectory,
                        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                        Os = Environment.OSVersion.VersionString,
                        MachineName = Environment.MachineName,
                        ProcessorCores = Environment.ProcessorCount,
                        MemoryAmount = Environment.WorkingSet
                    }
                };
                var response = _monitorServiceClient.Register(registerServiceRequest);
                if (response.Succeed)
                {
                    serviceId = response.ServiceId;
                    if (postAction != null)
                    {
                        postAction.Invoke();
                    }
                }
                else
                {
                    throw new MicroserviceClientException(MicroserviceClientCodes.FailedRegisterFromServer);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("There are some erros when trying to connect Service Management, please check stack trace.", ex);
            }
        }

        public void Stop(Action postAction)
        {
            try
            {
                _monitorServiceClient.Stop(new StopServiceRequest { ServiceId = serviceId });
                if (postAction != null)
                {
                    postAction.Invoke();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There are some erros when trying to notify stop behavior in Service Management, please check stack trace.", ex);
            }
        }

        public void Run(Action postAction)
        {
            try
            {
                _monitorServiceClient.Ready(new ReadyStateRequest { ServiceId = serviceId });
                if (postAction != null)
                {
                    postAction.Invoke();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There are some erros when trying to notify stop behavior in Service Management, please check stack trace.", ex);
            }
        }

        public async Task PushLog(LogCollectorRequest logCollectorRequest)
        {
            if (_loggerOptions.CurrentValue.NotifyOptions.Enable)
            {
                logCollectorRequest.ServiceId = serviceId;
                logCollectorRequest.ServiceName = _serviceOptions.CurrentValue.Name;
                try
                {
                    while (await _logCollectorChannel.GetChannel().Writer
                            .WaitToWriteAsync()
                            .ConfigureAwait(false))
                    {
                        _logCollectorChannel.GetChannel().Writer.TryWrite(logCollectorRequest);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("There are some erros when trying to notify log behavior in Service Management, please check stack trace.", ex);
                }
            }
        }

        public async Task PushHealthCheck(HealthCheckRequest healthCheckRequest)
        {
            if (_monitorOptions.CurrentValue.NotifyOptions.Enable)
            {
                try
                {
                    healthCheckRequest.ServiceId = serviceId;
                    await _monitoHeartBeatChannel.GetChannel().Writer.WriteAsync(healthCheckRequest);
                }
                catch (Exception ex)
                {
                    throw new Exception("There are some erros when trying to notify monitor behavior in Service Management, please check stack trace.", ex);
                }
            }
        }
    }
}
