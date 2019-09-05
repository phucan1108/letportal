using LetPortal.Portal.Models;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Databases
{
    public interface IDatabaseService
    {
        Task<ExecuteDynamicResultModel> ExecuteDynamic(string databaseId, string formattedString);
    }
}
