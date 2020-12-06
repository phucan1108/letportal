using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Microservices.Monitors;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Providers
{
    public class ServiceManagamentProvider : IServiceManagementProvider
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceManagamentProvider(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task CheckAndShutdownAllLostServices(int durationShutdown)
        {
            await _serviceRepository.UpdateShutdownStateForAllServices(durationShutdown);
        }

        public async Task CheckAndUpdateAllLostServices(int durationLost)
        {
            await _serviceRepository.UpdateLostStateForAllLosingServices(durationLost);
        }

        public Task<string[]> GetAllRunningServices()
        {
            return Task.FromResult(_serviceRepository.GetAsQueryable().Where(a => a.ServiceState == ServiceState.Run).Select(b => b.Id).ToArray());
        }

        public async Task<string> RegisterService(RegisterServiceRequest registerServiceRequest)
        {
            var service = new Service
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = registerServiceRequest.ServiceName,
                ServiceState = ServiceState.Start,
                IpAddress = registerServiceRequest.IpAddress,
                LoggerNotifyEnable = registerServiceRequest.LoggerNotifyEnable,
                HealthCheckNotifyEnable = registerServiceRequest.HealthCheckNotifyEnable,
                LastCheckedDate = DateTime.UtcNow,
                RegisteredDate = DateTime.UtcNow,
                RunningVersion = registerServiceRequest.Version,
                InstanceNo = await _serviceRepository.GetLastInstanceNoOfService(registerServiceRequest.ServiceName)
            };

            service.ServiceHardwareInfo = new Entities.ServiceHardwareInfo
            {
                ServiceId = service.Id,
                AllocationMemory = registerServiceRequest.ServiceHardwareInfo.MemoryAmount,
                Id = DataUtil.GenerateUniqueId(),
                Directory = registerServiceRequest.ServiceHardwareInfo.Directory,
                MachineName = registerServiceRequest.ServiceHardwareInfo.MachineName,
                Os = registerServiceRequest.ServiceHardwareInfo.Os,
                ProcessorCores = registerServiceRequest.ServiceHardwareInfo.ProcessorCores
            };

            service.CalculateRunningTime();

            await _serviceRepository.AddAsync(service);

            return service.Id;
        }

        public async Task ShutdownAllServices()
        {
            await _serviceRepository.ForceShutdownAllServices();
        }

        public async Task ShutdownService(string serviceId)
        {
            var service = await _serviceRepository.GetOneAsync(serviceId);

            service.ServiceState = ServiceState.Shutdown;
            service.LastCheckedDate = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(serviceId, service);
        }

        public async Task UpdateRunningState(string serviceId)
        {
            var service = await _serviceRepository.GetOneAsync(serviceId);

            service.ServiceState = ServiceState.Run;
            service.LastCheckedDate = DateTime.UtcNow;
            service.CalculateRunningTime();
            await _serviceRepository.UpdateAsync(serviceId, service);
        }
    }
}
