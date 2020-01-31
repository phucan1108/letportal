using System.Threading.Tasks;
using LetPortal.Core.Logger.Models;

namespace LetPortal.ServiceManagement.Providers
{
    public interface ILogEventProvider
    {
        Task AddLogEvent(PushLogModel pushLogModel);

        Task GatherAllLogs(string traceId);
    }
}
