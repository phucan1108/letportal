using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Repositories.Databases;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class ExtractingColumnSchemaHandler : IRequestHandler<ExtractingColumnSchemaRequest, ExtractingSchemaQueryModel>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public ExtractingColumnSchemaHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<ExtractingSchemaQueryModel> Handle(ExtractingColumnSchemaRequest request, CancellationToken cancellationToken)
        {
            // queryJsonString sample
            // { "$query" : { 
            //      "users": [
            //          "$match" : { 'username': 'A' }
            //      ]
            //  }}
            // Note: we just support aggreation framework only
            var result = new ExtractingSchemaQueryModel();

            var database = await _databaseRepository.GetOneAsync(request.GetQuery().DatabaseId);

            JObject parsingObject = JObject.Parse(request.GetQuery().QueryJsonString);

            var mongoClient = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource);

            var executionGroupTypes = parsingObject.Children().Select(a => (a as JProperty).Name);

            foreach (var executionGroupType in executionGroupTypes)
            {
                switch (executionGroupType)
                {
                    case "$query":
                        var collectionNames = parsingObject["$query"].Children().Select(a => (a as JProperty).Name);
                        foreach (var collectionName in collectionNames)
                        {
                            var mongoCollection = mongoClient.GetCollection<BsonDocument>(collectionName);
                            string collectionQuery = parsingObject["$query"][collectionName].ToString(Newtonsoft.Json.Formatting.Indented);
                            List<PipelineStageDefinition<BsonDocument, BsonDocument>> aggregatePipes = BsonSerializer.Deserialize<BsonDocument[]>(collectionQuery).Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();

                            IAggregateFluent<BsonDocument> aggregateFluent = mongoCollection.Aggregate();
                            foreach (PipelineStageDefinition<BsonDocument, BsonDocument> pipe in aggregatePipes)
                            {
                                aggregateFluent = aggregateFluent.AppendStage(pipe);
                            }

                            using (IAsyncCursor<BsonDocument> executingCursor = await aggregateFluent.ToCursorAsync())
                            {
                                while (executingCursor.MoveNext())
                                {
                                    // Important note: this query must have one row result for extracting params and filters
                                    BsonDocument currentDocument = executingCursor.Current.First();

                                    foreach (BsonElement element in currentDocument.Elements)
                                    {
                                        ColumnField columnField = new ColumnField
                                        {
                                            Name = element.Name,
                                            DisplayName = element.Name,
                                            FieldType = GetTypeByBsonDocument(element.Value)
                                        };

                                        result.ColumnFields.Add(columnField);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private string GetTypeByBsonDocument(BsonValue bsonValue)
        {
            if (bsonValue.IsBoolean)
            {
                return "boolean";
            }
            if (bsonValue.IsInt32
                || bsonValue.IsInt64
                || bsonValue.IsNumeric
                || bsonValue.IsDecimal128
                || bsonValue.IsDouble)
            {
                return "number";
            }
            if (bsonValue.IsValidDateTime)
            {
                return "datetime";
            }
            if (bsonValue.IsBsonArray)
            {
                return "list";
            }
            if (bsonValue.IsBsonDocument)
            {
                return "document";
            }
            return "string";
        }
    }
}
