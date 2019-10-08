using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Databases;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExtractionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString);
    }
}
