using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Pages;

namespace LetPortal.Portal.Providers.Pages
{
    public interface IPageServiceProvider
    {
        Task<IEnumerable<Page>> GetPagesByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> ComparePages(IEnumerable<Page> pages);

        Task<List<ShortPageModel>> GetAllPages();

        Task ForceUpdatePages(IEnumerable<Page> pages);

        Task<IEnumerable<LanguageKey>> GetPageLanguages(string pageId);

        Task<IEnumerable<Page>> GetByAppId(string appId);

        Task DeleteByAppIdAsync(string appId);

        Task<bool> CheckPageExist(Expression<Func<Page, bool>> expression);
    }
}
