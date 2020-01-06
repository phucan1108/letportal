using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Pages
{
    public class InternalPageServiceProvider : IPageServiceProvider
    {
        private readonly IPageRepository _pageRepository;

        public InternalPageServiceProvider(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> ComparePages(IEnumerable<Page> pages)
        {
            var results = new List<ComparisonResult>();
            foreach(var page in pages)
            {
                results.Add(await _pageRepository.Compare(page));
            }
            return results;
        }

        public async Task ForceUpdatePages(IEnumerable<Page> pages)
        {
            foreach(var page in pages)
            {
                await _pageRepository.ForceUpdateAsync(page.Id, page);
            }
        }

        public async Task<List<ShortPageModel>> GetAllPages()
        {
            return await _pageRepository.GetAllShortPagesAsync();
        }

        public async Task<IEnumerable<Page>> GetPagesByIds(IEnumerable<string> ids)
        {
            return await _pageRepository.GetAllByIdsAsync(ids);
        }
    }
}
