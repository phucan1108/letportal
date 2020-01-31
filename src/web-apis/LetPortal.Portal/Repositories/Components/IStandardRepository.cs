using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IStandardRepository : IGenericRepository<StandardComponent>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortStandards(string keyWord = null);
    }
}
