using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

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
