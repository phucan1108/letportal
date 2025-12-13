using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Databases;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories
{
    public class DatabaseMongoRepository : MongoGenericRepository<DatabaseConnection>, IDatabaseRepository
    {
        public DatabaseMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneDatabase = await GetOneAsync(cloneId);
            cloneDatabase.Id = DataUtil.GenerateUniqueId();
            cloneDatabase.Name = cloneName;
            cloneDatabase.DisplayName += " Clone";
            await AddAsync(cloneDatabase);
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortDatatabases(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var filterBuilder = Builders<DatabaseConnection>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var databases = Collection.Find(filterBuilder).ToList();
                return Task.FromResult(databases?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }));
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }
    }
}
