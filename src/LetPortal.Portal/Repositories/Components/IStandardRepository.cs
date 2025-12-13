using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Components
{
    public interface IStandardRepository : IGenericRepository<StandardComponent>
    {          

        Task CloneAsync(string cloneId, string cloneName);

        Task<IEnumerable<ShortEntityModel>> GetShortStandards(string keyWord = null);

        Task<IEnumerable<ShortEntityModel>> GetShortArrayStandards(string keyWord = null);

        Task<IEnumerable<ShortEntityModel>> GetShortTreeStandards(string keyWord = null);

        Task<StandardComponent> GetOneForRenderAsync(string id);

        Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string standardId);

        Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId);
    }
}
