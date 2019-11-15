using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Repositories.Abstractions
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<int> GetLastInstanceNoOfService(string serviceName);

        Task UpdateShutdownStateForAllServices(int durationShutdown);

        Task UpdateLostStateForAllLosingServices(int durationLost);

        Task ForceShutdownAllServices();
    }
}
