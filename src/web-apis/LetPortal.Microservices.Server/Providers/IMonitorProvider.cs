using System.Threading.Tasks;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Server.Providers
{
    public interface IMonitorProvider
    {
        Task AddMonitorPulse(HealthCheckRequest healthCheckRequest);
    }
}
