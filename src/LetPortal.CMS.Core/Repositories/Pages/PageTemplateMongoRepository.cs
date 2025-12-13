using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Pages
{
    public class PageTemplateMongoRepository : MongoGenericRepository<PageTemplate>, IPageTemplateRepository
    {
        public PageTemplateMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
