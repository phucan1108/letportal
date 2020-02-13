using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartMongoRepository : MongoGenericRepository<Chart>, IChartRepository
    {
        public ChartMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<Chart>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<Chart>.Filter.Eq("_t", typeof(Chart).Name);
                var combineFilter = Builders<Chart>.Filter.And(discriminatorFilter, regexFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }
    }
}
