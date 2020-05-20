using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Pages
{
    public interface IPageRepository : IGenericRepository<Page>
    {
        Task<Page> GetOneByNameForRenderAsync(string name);

        Task<Page> GetOneByNameAsync(string name);

        Task<List<ShortPageModel>> GetAllShortPagesAsync();

        Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync();

        Task<IEnumerable<ShortEntityModel>> GetShortPages(string keyWord = null);

        Task CloneAsync(string cloneId, string cloneName);

        Task<IEnumerable<LanguageKey>> GetLanguageKeys(string pageId);

        Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId);
    }
}
