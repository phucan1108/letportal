using LetPortal.Portal.Constants;
using LetPortal.Portal.Databases.Models;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Models;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class ExecuteDynamicRawToFetchAllEntitiesSchemasHandler : IRequestHandler<ExecuteDynamicRawToFetchAllEntitiesSchemasRequest, ParsingRawQueryModel>
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public ExecuteDynamicRawToFetchAllEntitiesSchemasHandler(IDatabaseServiceProvider databaseServiceProvider, IEntitySchemaRepository entitySchemaRepository)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _entitySchemaRepository = entitySchemaRepository;
        }

        public async Task<ParsingRawQueryModel> Handle(ExecuteDynamicRawToFetchAllEntitiesSchemasRequest request, CancellationToken cancellationToken)
        {
            string databaseId = request.GetQuery().DatabaseId;
            string rawQuery = request.GetQuery().RawQuery;
            bool isAggregation = request.GetQuery().IsAggregation;

            DatabaseConnection database = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(databaseId);

            IMongoDatabase requestingDatabaseConnection = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource);

            List<ParsingParam> parsingParams = ExtractParams(rawQuery);

            string executionQuery = rawQuery;

            string replacingQueryWithName = rawQuery;

            if (parsingParams.Count > 0)
            {
                executionQuery = ReplaceParamByDefaultValue(rawQuery, parsingParams);

                replacingQueryWithName = ReplaceParamWithDefaultName(rawQuery, parsingParams);
            }

            JObject parsingObject = JObject.Parse(executionQuery);

            IEnumerable<string> collectionNames = parsingObject.Properties().Select(a => a.Name).ToList();

            List<EntitySchema> entitySchemas = new List<EntitySchema>();

            foreach (string collectionName in collectionNames)
            {
                IMongoCollection<BsonDocument> mongoCollection = requestingDatabaseConnection.GetCollection<BsonDocument>(collectionName);

                string collectionQuery = parsingObject[collectionName].ToString(Newtonsoft.Json.Formatting.Indented);

                EntitySchema entitySchema = new EntitySchema
                {
                    DatabaseId = databaseId,
                    DisplayName = collectionName,
                    Name = collectionName
                };

                if (isAggregation)
                {
                    PipelineDefinition<BsonDocument, BsonDocument> aggregationPipes = BsonSerializer.Deserialize<BsonDocument[]>(collectionQuery).ToList();
                    using (IAsyncCursor<BsonDocument> executingCursor = await mongoCollection.AggregateAsync(aggregationPipes))
                    {
                        while (executingCursor.MoveNext())
                        {
                            // Important note: this query must have one row result for extracting params and filters
                            BsonDocument currentDocument = executingCursor.Current.First();

                            foreach (BsonElement element in currentDocument.Elements)
                            {
                                EntityField entityField = new EntityField
                                {
                                    Name = element.Name,
                                    DisplayName = element.Name,
                                    FieldType = GetTypeByBsonDocument(element.Value)
                                };

                                entitySchema.EntityFields.Add(entityField);
                            }
                        }
                    }
                }
                else
                {
                    BsonDocument collectionQueryBson = BsonDocument.Parse(collectionQuery);
                    using (IAsyncCursor<BsonDocument> executingCursor = await mongoCollection.Find(collectionQueryBson).ToCursorAsync())
                    {
                        while (executingCursor.MoveNext())
                        {
                            // Important note: this query must have one row result for extracting params and filters
                            BsonDocument currentDocument = executingCursor.Current.First();

                            foreach (BsonElement element in currentDocument.Elements)
                            {
                                EntityField entityField = new EntityField
                                {
                                    Name = element.Name,
                                    DisplayName = element.Name,
                                    FieldType = GetTypeByBsonDocument(element.Value)
                                };

                                entitySchema.EntityFields.Add(entityField);
                            }
                        }
                    }
                }

                entitySchemas.Add(entitySchema);
            }

            return new ParsingRawQueryModel
            {
                EntityFields = entitySchemas.SelectMany(a => a.EntityFields).ToList()
            };
        }

        private List<ParsingParam> ExtractParams(string rawQuery)
        {
            List<string> extractingParameters = new List<string>();
            Regex regex = new Regex(@"\(([^\}]+)\)");
            MatchCollection matches = regex.Matches(rawQuery);
            foreach (Match match in matches) //you can loop through your matches like this
            {
                string valueWithoutBrackets = match.Groups[1].Value; // name, name@gmail.com
                extractingParameters.Add(valueWithoutBrackets);
            }

            List<ParsingParam> parsingParams = new List<ParsingParam>();
            if (extractingParameters.Count > 0)
            {
                foreach (string param in extractingParameters)
                {
                    string[] splittedParams = param.Split("|");

                    parsingParams.Add(new ParsingParam { Name = splittedParams[0], DefaultValue = splittedParams[1], ParamType = splittedParams[2] });
                }
            }

            return parsingParams;
        }

        private string ReplaceParamByDefaultValue(string rawQuery, IEnumerable<ParsingParam> parsingParams)
        {
            if (parsingParams != null && parsingParams.Any())
            {
                foreach (ParsingParam param in parsingParams)
                {
                    rawQuery = rawQuery.Replace(param.GetFullParam(), param.DefaultValue);
                }
            }

            return rawQuery;
        }

        private string ReplaceParamWithDefaultName(string rawQuery, IEnumerable<ParsingParam> parsingParams)
        {
            if (parsingParams != null && parsingParams.Any())
            {
                foreach (ParsingParam param in parsingParams)
                {
                    rawQuery = rawQuery.Replace(param.GetFullParam(), param.GetNameParam());
                }
            }

            return rawQuery;
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
