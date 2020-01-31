using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Databases
{
    public interface IDatabaseRepository : IGenericRepository<DatabaseConnection>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortDatatabases(string keyWord = null);
    }
}
