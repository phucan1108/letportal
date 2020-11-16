using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Pages
{
    public class PageMongoRepository : MongoGenericRepository<Page>, IPageRepository
    {
        public PageMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
