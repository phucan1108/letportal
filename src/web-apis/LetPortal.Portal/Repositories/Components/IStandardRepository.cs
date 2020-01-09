using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IStandardRepository : IGenericRepository<StandardComponent>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortStandards(string keyWord = null);
    }
}
