using System;
using System.Threading.Tasks;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Client
{
    public interface IServiceContext
    {
        Task PushHealthCheck(HealthCheckRequest healthCheckRequest);

        Task PushLog(LogCollectorRequest logCollectorRequest);

        void Start(Action postAction);

        void Run(Action postAction);

        void Stop(Action postAction);
    }
}
