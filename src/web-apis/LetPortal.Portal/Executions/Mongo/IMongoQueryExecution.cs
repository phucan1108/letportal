using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Models.Databases;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetPortal.Portal.Executions.Mongo
{
    public interface IMongoQueryExecution
    {
        Task<dynamic> ExecuteAsync(
            IMongoDatabase mongoDatabase,
            string formattedString,
            string mappingProjection,
            IEnumerable<ExecuteParamModel> parameters,
            List<PipelineStageDefinition<BsonDocument, BsonDocument>> filterStages = null);
    }
}
