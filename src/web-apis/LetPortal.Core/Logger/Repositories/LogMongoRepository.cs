using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace LetPortal.Core.Logger.Repositories
{
    public class LogMongoRepository : ILogRepository
    {
        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;
        public LogMongoRepository(IOptionsMonitor<LoggerOptions> loggerOptions)
        {
            _loggerOptions = loggerOptions;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<IEnumerable<string>> GetAllLogs(string serviceName, string userSessionId, string traceId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            //var option = _loggerOptions.CurrentValue;
            //switch(option.Type)
            //{
            //    case LoggerStorageType.MongoDB:
            //        var databaseName = MongoUrl.Create(option.MongoOptions.ConnectionString).DatabaseName;
            //        var mongoDatabase = new MongoClient(option.MongoOptions.ConnectionString).GetDatabase(databaseName);
            //        var mongoCollection = mongoDatabase.GetCollection<BsonDocument>(option.MongoOptions.CollectionName);
            //        FieldDefinition<BsonDocument, string> traceIdField = "Properties.TraceId";
            //        FieldDefinition<BsonDocument, string> userSessionIdField = "Properties.UserSessionId";
            //        FieldDefinition<BsonDocument, string> serviceNameField = "Properties.ServiceName";
            //        var traceIdFilter = Builders<BsonDocument>.Filter.Eq(traceIdField, traceId);
            //        var userSessionFilter = Builders<BsonDocument>.Filter.Eq(userSessionIdField, userSessionId);
            //        var serviceNameFilter = Builders<BsonDocument>.Filter.Eq(serviceNameField, serviceName);
            //        var combineAndFilter = Builders<BsonDocument>.Filter.And(serviceNameFilter, userSessionFilter, traceIdFilter);
            //        var logs = await mongoCollection.Find(combineAndFilter).ToListAsync();
            //        if(logs != null)
            //        {
            //            return from log in logs
            //                   select log["RenderedMessage"].AsString;
            //        }
            //        break;
            //    default:
            //        break;
            //}

            return null;
        }
    }
}
