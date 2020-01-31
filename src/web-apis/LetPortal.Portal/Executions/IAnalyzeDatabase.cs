using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IAnalyzeDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(DatabaseConnection databaseConnection);
    }
}
