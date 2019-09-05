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

        public async Task CreateAsync(Page page)
        {
            await _pageRepository.AddAsync(page);
        }

        public async Task DeleteAsync(string id)
        {
            await _pageRepository.DeleteAsync(id);
        }

        public async Task<List<ShortPageModel>> GetAllPages()
        {
            return await _pageRepository.GetAllShortPages();
        }

        public async Task<Page> GetOne(string id)
        {
            return await _pageRepository.GetOneAsync(id);
        }

        public async Task UpdateAsync(string id, Page page)
        {
            await _pageRepository.UpdateAsync(id, page);
        }
    }
}
