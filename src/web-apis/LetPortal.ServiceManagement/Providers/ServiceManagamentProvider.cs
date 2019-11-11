using LetPortal.Core.Services.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories;
using System;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
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

        public async Task<string> RegisterService(RegisterServiceModel registerServiceModel)
        {
            var service = new Service
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = registerServiceModel.ServiceName,
                ServiceState = ServiceState.Start,
                IpAddress = registerServiceModel.IpAddress,
                ServiceHardwareInfo = registerServiceModel.ServiceHardwareInfo,
                LoggerNotifyEnable = registerServiceModel.LoggerNotifyEnable,
                HealthCheckNotifyEnable = registerServiceModel.HealthCheckNotifyEnable,
                LastCheckingDate = DateTime.UtcNow,
                RunningVersion = registerServiceModel.Version,
                InstanceNo = await _serviceRepository.GetLastInstanceNoOfService(registerServiceModel.ServiceName)
            };

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
            service.LastCheckingDate = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(serviceId, service);
        }

        public async Task UpdateRunningState(string serviceId)
        {
            var service = await _serviceRepository.GetOneAsync(serviceId);

            service.ServiceState = ServiceState.Run;
            service.LastCheckingDate = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(serviceId, service);
        }
    }
}
