using LetPortal.Core.Logger.Models;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public interface ILogEventProvider
    {
        Task AddLogEvent(PushLogModel pushLogModel);
    }
}
