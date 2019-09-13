using LetPortal.Core.Persistences;
using LetPortal.Portal.Models.Databases;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExtractionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExtractingSchemaQueryModel> Extract(object database, string formattedString);
    }
}
