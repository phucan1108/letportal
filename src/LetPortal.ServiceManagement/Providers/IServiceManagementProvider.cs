using System.Threading.Tasks;
using LetPortal.Core.Services.Models;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IServiceManagementProvider
    {
        Task<string[]> GetAllRunningServices();

        Task<string> RegisterService(RegisterServiceModel registerServiceModel);

        Task CheckAndUpdateAllLostServices(int durationLost);

        Task CheckAndShutdownAllLostServices(int durationShutdown);

        Task UpdateRunningState(string serviceId);

        Task ShutdownService(string serviceId);

        Task ShutdownAllServices();
    }
}
