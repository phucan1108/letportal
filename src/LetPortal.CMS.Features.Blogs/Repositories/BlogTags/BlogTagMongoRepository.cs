using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Persistences;
using MongoDB.Driver;

namespace LetPortal.CMS.Features.Blogs.Repositories.BlogTags
{
    public class BlogTagMongoRepository : MongoGenericRepository<BlogTag>, IBlogTagRepository
    {
        public BlogTagMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<IEnumerable<BlogTag>> GetTags(IEnumerable<string> tagIds)
        {
            var filter = Builders<BlogTag>.Filter.In(a => a.Id, tagIds);

            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<BlogTag>> GetTagsByName(IEnumerable<string> tags)
        {
            var filter = Builders<BlogTag>.Filter.In(a => a.Tag, tags);

            return await Collection.Find(filter).ToListAsync();
        }
    }
}
