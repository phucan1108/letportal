using System;
using System.Reflection;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Monitors;
using LetPortal.Microservices.Server.Options;
using LetPortal.Microservices.Server.Providers;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Server.Services
{
    public class ServerServiceContext : IServiceContext
    {
        private static string serviceId;

        private static string runningVersion;

        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;

        private readonly IOptionsMonitor<MonitorOptions> _monitorOptions;

        private readonly IMonitorProvider _monitorProvider;

        private readonly IServiceManagementProvider _serviceManagementProvider;

        private readonly ILogEventProvider _logEventProvider;

        private readonly SelfServerOptions _selfOptions;

        public ServerServiceContext(
            SelfServerOptions selfOptions,
            IOptionsMonitor<LoggerOptions> loggerOptions,
            IOptionsMonitor<MonitorOptions> monitorOptions,
            IServiceManagementProvider serviceManagementProvider,
            IMonitorProvider monitorProvider,
            ILogEventProvider logEventProvider)
        {
            _loggerOptions = loggerOptions;
            _monitorOptions = monitorOptions;
            _serviceManagementProvider = serviceManagementProvider;
            _monitorProvider = monitorProvider;
            _logEventProvider = logEventProvider;
            _selfOptions = selfOptions;
            runningVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void PushHealthCheck(HealthCheckRequest healthCheckRequest)
        {
            healthCheckRequest.ServiceId = serviceId;
            healthCheckRequest.ServiceName = _selfOptions.ServerName;
            var monitorTask = _monitorProvider.AddMonitorPulse(healthCheckRequest);
            monitorTask.Wait();
        }

        public void Run(Action postAction)
        {
            var serviceTask = _serviceManagementProvider.UpdateRunningState(serviceId);
            serviceTask.ConfigureAwait(false);
            serviceTask.Wait();
        }

        public void Start(Action postAction)
        {
            var registerServiceRequest = new RegisterServiceRequest
            {
                ServiceName = _selfOptions.ServerName,
                Version = runningVersion,
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
            var selfRegisterTask = _serviceManagementProvider.RegisterService(registerServiceRequest);
            selfRegisterTask.Wait();
            serviceId = selfRegisterTask.Result;
            Console.WriteLine("Self Server Id " + serviceId);
        }

        public void Stop(Action postAction)
        {
            var serviceTask = _serviceManagementProvider.ShutdownService(serviceId);
            serviceTask.Wait();
        }

        public void PushLog(LogCollectorRequest logCollectorRequest)
        {
            logCollectorRequest.ServiceId = serviceId;
            logCollectorRequest.ServiceName = _selfOptions.ServerName;
            var logTask = _logEventProvider.AddLogEvent(logCollectorRequest);
            logTask.Wait();
        }
    }
}
