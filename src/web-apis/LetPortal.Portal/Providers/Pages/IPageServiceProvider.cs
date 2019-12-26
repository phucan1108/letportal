using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Pages
{
    public interface IPageServiceProvider
    {
        Task<IEnumerable<Page>> GetPagesByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> ComparePages(IEnumerable<Page> pages);

        Task<Page> GetOne(string id);

        Task<List<ShortPageModel>> GetAllPages();

        Task CreateAsync(Page page);

        Task UpdateAsync(string id, Page page);

        Task DeleteAsync(string id);
    }
}
