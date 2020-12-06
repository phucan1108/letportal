using System.Threading.Tasks;
using Grpc.Core;
using LetPortal.Core.Logger;
using LetPortal.Microservices.Monitors;
using LetPortal.Microservices.Server.Providers;

namespace LetPortal.Core.Microservices.Configurations.Server
{
    public class ServiceMonitorService : MonitorService.MonitorServiceBase
    {
        private readonly IServiceLogger<ServiceMonitorService> _logger;

        private readonly IMonitorProvider _monitorProvider;

        private readonly IServiceManagementProvider _serviceManagementProvider;

        public ServiceMonitorService(
            IServiceLogger<ServiceMonitorService> logger,
            IMonitorProvider monitorProvider,
            IServiceManagementProvider serviceManagementProvider)
        {
            _logger = logger;
            _monitorProvider = monitorProvider;
            _serviceManagementProvider = serviceManagementProvider;
        }

        public override async Task<HealthCheckResponse> Push(IAsyncStreamReader<HealthCheckRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {                   
                await _monitorProvider.AddMonitorPulse(requestStream.Current);
            }

            return new HealthCheckResponse { Succeed = true };
        }

        public override async Task<RegisterServiceResponse> Register(RegisterServiceRequest request, ServerCallContext context)
        {
            var result = await _serviceManagementProvider.RegisterService(request);

            return new RegisterServiceResponse { Succeed = true, ServiceId = result };
        }

        public override async Task<ReadyStateResponse> Ready(ReadyStateRequest request, ServerCallContext context)
        {
            await _serviceManagementProvider.UpdateRunningState(request.ServiceId);
            return new ReadyStateResponse { Succeed = true };
        }

        public override async Task<StopServiceResponse> Stop(StopServiceRequest request, ServerCallContext context)
        {
            await _serviceManagementProvider.ShutdownService(request.ServiceId);
            return new StopServiceResponse { Succeed = true };
        }
    }
}
