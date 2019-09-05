using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Core.Logger.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<string>> GetAllLogs(string serviceName, string userSessionId, string traceId);
    }
}
