using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Databases;

namespace LetPortal.Portal.Executions
{
    public interface IExtractionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString, IEnumerable<ExecuteParamModel> parameters);
    }
}
