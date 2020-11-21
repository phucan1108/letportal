using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExtractionDatabase : IExtractionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        private readonly IOptionsMonitor<MongoOptions> _mongoOptions;

        public MongoExtractionDatabase(IOptionsMonitor<MongoOptions> options)
        {
            _mongoOptions = options;
        }

        public async Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            // queryJsonString sample
            // { "$query" : { 
            //      "users": [
            //          "$match" : { 'username': 'A' }
            //      ]
            //  }}
            // Note: we just support aggreation framework only
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.RemoveQuotes)
                    {
                        formattedString = formattedString.Replace("\"{{" + param.Name + "}}\"", param.ReplaceValue);
                    }
                    else
                    {
                        formattedString = formattedString.Replace("{{" + param.Name + "}}", param.ReplaceValue);
                    }

                }
            }

            formattedString = _mongoOptions.CurrentValue.EliminateDoubleQuotes(formattedString);
            var result = new ExtractingSchemaQueryModel();

            var parsingBson = BsonSerializer.Deserialize<BsonDocument>(formattedString);

            var mongoDatabase = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource);

            var executionGroupType = parsingBson.First().Name;

            switch (executionGroupType)
            {
                case Constants.QUERY_KEY:
                    var mongoCollection = GetCollection(mongoDatabase, parsingBson, Constants.QUERY_KEY);
                    var aggregatePipes = parsingBson[Constants.QUERY_KEY][0].AsBsonArray.Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();
                    var aggregateFluent = mongoCollection.Aggregate();
                    foreach (var pipe in aggregatePipes)
                    {
                        aggregateFluent = aggregateFluent.AppendStage(pipe);
                    }
                    using (var executingCursor = await aggregateFluent.ToCursorAsync())
                    {
                        while (executingCursor.MoveNext())
                        {
                            // Important note: this query must have one row result for extracting params and filters
                            var currentDocument = executingCursor.Current.First();

                            foreach (var element in currentDocument.Elements)
                            {
                                var columnField = new ColumnField
                                {
                                    Name = element.Name,
                                    DisplayName = element.Name,
                                    FieldType = GetTypeByBsonDocument(element.Value)
                                };

                                result.ColumnFields.Add(columnField);
                            }

                            break;
                        }
                    }
                    break;
                default:
                    break;
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

        private static IMongoCollection<BsonDocument> GetCollection(IMongoDatabase mongoDatabase, BsonDocument parsingBson, string operatorName)
        {
            var collectionName = parsingBson[operatorName].AsBsonDocument.First().Name;
            return mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }
}
