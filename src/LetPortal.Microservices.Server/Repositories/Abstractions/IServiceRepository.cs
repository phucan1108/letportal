using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;

namespace LetPortal.Microservices.Server.Repositories.Abstractions
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<int> GetLastInstanceNoOfService(string serviceName);

        Task UpdateShutdownStateForAllServices(int durationShutdown);

        Task UpdateLostStateForAllLosingServices(int durationLost);

        Task ForceShutdownAllServices();
    }
}
