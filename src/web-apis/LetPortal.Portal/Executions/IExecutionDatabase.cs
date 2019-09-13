using LetPortal.Core.Persistences;
using LetPortal.Portal.Models;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExecuteDynamicResultModel> Execute(object database, string formattedString);
    }
}
