using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.CMS.Features.Blogs.Repositories.Blogs
{
    public class BlogMongoRepository : MongoGenericRepository<Blog>, IBlogRepository
    {
        public BlogMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<Blog> GetByUrlPathAsync(string urlPath)
        {
            return await Collection.AsQueryable().FirstAsync(a => a.UrlPath == urlPath);
        }
    }
}
