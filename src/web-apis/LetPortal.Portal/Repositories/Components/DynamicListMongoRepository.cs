using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Components
{
    public class DynamicListMongoRepository : MongoGenericRepository<DynamicList>, IDynamicListRepository
    {
        public DynamicListMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null)
        {
            if(!string.IsNullOrEmpty(keyWord))
            {
                var filterBuilder = Builders<DynamicList>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));

                return Task.FromResult(Collection.Find(filterBuilder).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }
    }
}
