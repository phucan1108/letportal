using LetPortal.Core.Monitors.Models;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IMonitorProvider
    {
        Task AddMonitorPulse(PushHealthCheckModel pushHealthCheckModel);
    }
}
