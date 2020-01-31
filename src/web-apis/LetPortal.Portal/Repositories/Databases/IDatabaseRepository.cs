using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Databases
{
    public interface IDatabaseRepository : IGenericRepository<DatabaseConnection>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortDatatabases(string keyWord = null);
    }
}
