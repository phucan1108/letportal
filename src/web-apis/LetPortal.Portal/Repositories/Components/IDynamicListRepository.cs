using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IDynamicListRepository : IGenericRepository<DynamicList>
    {
        Task CloneAsync(string cloneId, string cloneName);

        Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null);
    }
}
