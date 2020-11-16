using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Themes
{
    public class ThemeMongoRepository : MongoGenericRepository<Theme>, IThemeRepository
    {
        public ThemeMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }


    }
}
