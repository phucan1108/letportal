using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Portal.Repositories.Components
{
    public class CompositeControlMongoRepository : MongoGenericRepository<CompositeControl>, ICompositeControlRepository
    {
        public CompositeControlMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortEntities(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var regexFilter = Builders<CompositeControl>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyword, "i"));  
                return Task.FromResult(Collection.Find(regexFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {
                return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
        }
    }
}
