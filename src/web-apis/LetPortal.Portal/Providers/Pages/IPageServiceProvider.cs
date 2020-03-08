using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;

namespace LetPortal.Portal.Providers.Pages
{
    public interface IPageServiceProvider
    {
        Task<IEnumerable<Page>> GetPagesByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> ComparePages(IEnumerable<Page> pages);

        Task<List<ShortPageModel>> GetAllPages();

        Task ForceUpdatePages(IEnumerable<Page> pages);
    }
}
