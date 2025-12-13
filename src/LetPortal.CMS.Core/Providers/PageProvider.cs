using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Repositories.Pages;

namespace LetPortal.CMS.Core.Providers
{
    public class PageProvider : IPageProvider
    {
        private readonly IPageRepository _pageRepository;

        private readonly IPageTemplateRepository _pageTemplateRepository;

        private readonly IPageVersionRepository _pageVersionRepository;

        public PageProvider(
            IPageRepository pageRepository,
            IPageTemplateRepository pageTemplateRepository,
            IPageVersionRepository pageVersionRepository)
        {
            _pageRepository = pageRepository;
            _pageTemplateRepository = pageTemplateRepository;
            _pageVersionRepository = pageVersionRepository;
        }

        public async Task<Page> LoadPage(string pageId)
        {
            return await _pageRepository.GetOneAsync(pageId);
        }

        public async Task<PageTemplate> LoadPageTemplate(string pageTemplateId)
        {
            return await _pageTemplateRepository.GetOneAsync(pageTemplateId);
        }

        public async Task<PageVersion> LoadPageVersion(string pageVersionId)
        {
            return await _pageVersionRepository.GetOneAsync(pageVersionId);
        }
    }
}
