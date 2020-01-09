using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IDynamicListRepository : IGenericRepository<DynamicList>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null);
    }
}
