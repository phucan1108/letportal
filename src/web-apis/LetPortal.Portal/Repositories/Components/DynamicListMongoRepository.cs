using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
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

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneList = await GetOneAsync(cloneId);
            cloneList.Id = DataUtil.GenerateUniqueId();
            cloneList.Name = cloneName;
            cloneList.DisplayName += " Clone";
            await AddAsync(cloneList);
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<DynamicList>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<DynamicList>.Filter.Eq("_t", typeof(DynamicList).Name);
                var combineFilter = Builders<DynamicList>.Filter.And(discriminatorFilter, regexFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }
    }
}
