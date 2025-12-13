using System;
using LetPortal.Core.Logger;
using LetPortal.Core.Logger.Models;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Services;
using LetPortal.Core.Services.Models;
using LetPortal.ServiceManagement.Providers;
using Microsoft.Extensions.Options;

namespace LetPortal.ServiceManagement.Services
{
    public class InternalServiceContext : IServiceContext
    {
        private static string serviceId;

        private readonly IOptionsMonitor<ServiceOptions> _serviceOptions;

        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;

        private readonly IMonitorProvider _monitorProvider;

        private readonly IServiceManagementProvider _serviceManagementProvider;

        private readonly ILogEventProvider _logEventProvider;

        public InternalServiceContext(
            IOptionsMonitor<ServiceOptions> serviceOptions,
            IOptionsMonitor<LoggerOptions> loggerOptions,
            IServiceManagementProvider serviceManagementProvider,
            IMonitorProvider monitorProvider,
            ILogEventProvider logEventProvider)
        {
            _serviceOptions = serviceOptions;
            _loggerOptions = loggerOptions;
            _serviceManagementProvider = serviceManagementProvider;
            _monitorProvider = monitorProvider;
            _logEventProvider = logEventProvider;
        }

        public void PushHealthCheck(PushHealthCheckModel pushHealthCheckModel)
        {
            pushHealthCheckModel.ServiceId = serviceId;
            pushHealthCheckModel.ServiceName = _serviceOptions.CurrentValue.Name;
            var monitorTask = _monitorProvider.AddMonitorPulse(pushHealthCheckModel);
            monitorTask.Wait();
        }

        public void PushLog(PushLogModel pushLogModel)
        {
            pushLogModel.RegisteredServiceId = serviceId;
            pushLogModel.ServiceName = _serviceOptions.CurrentValue.Name;
            var logTask = _logEventProvider.AddLogEvent(pushLogModel);
            logTask.Wait();
        }

        public void Run(Action postAction)
        {
            var serviceTask = _serviceManagementProvider.UpdateRunningState(serviceId);
            serviceTask.Wait();
        }

        public void Start(Action postAction)
        {
            var serviceTask = _serviceManagementProvider.RegisterService(new Core.Services.Models.RegisterServiceModel
            {
                ServiceName = _serviceOptions.CurrentValue.Name,
                Version = _serviceOptions.CurrentValue.Version,
                ServiceHardwareInfo = new ServiceHardwareInfo
                {
                    Directory = Environment.CurrentDirectory,
                    MachineName = Environment.MachineName,
                    Os = Environment.OSVersion.VersionString,
                    ProcessorCores = Environment.ProcessorCount
                },
                LoggerNotifyEnable = _loggerOptions.CurrentValue.NotifyOptions.Enable
            });

            serviceTask.Wait();
            serviceId = serviceTask.Result;
        }

        public void Stop(Action postAction)
        {
            var serviceTask = _serviceManagementProvider.ShutdownService(serviceId);
            serviceTask.Wait();
        }
    }
}
