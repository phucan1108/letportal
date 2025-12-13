using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Services.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

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

        public Task<string[]> GetAllRunningServices()
        {
            return Task.FromResult(_serviceRepository.GetAsQueryable().Where(a => a.ServiceState == ServiceState.Run).Select(b => b.Id).ToArray());
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
                LastCheckedDate = DateTime.UtcNow,
                RegisteredDate = DateTime.UtcNow,
                RunningVersion = registerServiceModel.Version,
                InstanceNo = await _serviceRepository.GetLastInstanceNoOfService(registerServiceModel.ServiceName)
            };

            service.ServiceHardwareInfo.ServiceId = service.Id;
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
