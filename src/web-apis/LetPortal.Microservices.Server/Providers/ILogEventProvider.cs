using System.Threading.Tasks;
using LetPortal.Microservices.LogCollector;

namespace LetPortal.Microservices.Server.Providers
{
    public interface ILogEventProvider
    {
        Task AddLogEvent(LogCollectorRequest logCollectorRequest);

        Task GatherAllLogs(string traceId);
    }
}
