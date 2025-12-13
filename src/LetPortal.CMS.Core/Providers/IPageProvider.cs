using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Providers
{
    public interface IPageProvider
    {
        Task<Page> LoadPage(string pageId);

        Task<PageTemplate> LoadPageTemplate(string pageTemplateId);

        Task<PageVersion> LoadPageVersion(string pageVersionId);
    }
}
