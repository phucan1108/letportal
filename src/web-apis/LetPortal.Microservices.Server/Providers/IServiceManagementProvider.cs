using System.Threading.Tasks;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Server.Providers
{
    public interface IServiceManagementProvider
    {
        Task<string[]> GetAllRunningServices();

        Task<string> RegisterService(RegisterServiceRequest registerServiceRequest);

        Task CheckAndUpdateAllLostServices(int durationLost);

        Task CheckAndShutdownAllLostServices(int durationShutdown);

        Task UpdateRunningState(string serviceId);

        Task ShutdownService(string serviceId);

        Task ShutdownAllServices();
    }
}
