using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Server
{
    public interface IServiceContext
    {
        void PushHealthCheck(HealthCheckRequest healthCheckRequest);

        void PushLog(LogCollectorRequest logCollectorRequest);

        void Start(Action postAction);

        void Run(Action postAction);

        void Stop(Action postAction);
    }
}
