using LetPortal.Core.Logger.Models;
using LetPortal.Core.Monitors.Models;
using System;

namespace LetPortal.Core.Services
{
    public interface IServiceContext
    {
        void PushHealthCheck(PushHealthCheckModel pushHealthCheckModel);

        void PushLog(PushLogModel pushLogModel);

        void Start(Action postAction);

        void Run(Action postAction);

        void Stop(Action postAction);
    }
}
