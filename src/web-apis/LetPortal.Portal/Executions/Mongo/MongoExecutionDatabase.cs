using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public async Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            try
            {
                formattedString = StringUtil.ReplaceDoubleCurlyBraces(formattedString, parameters.Select(a => new Tuple<string, string, bool>(a.Name, a.ReplaceValue, a.RemoveQuotes)));
                var result = new ExecuteDynamicResultModel { IsSuccess = true };
                var query = EliminateRedundantFormat(formattedString);
                var mongoDatabase = new MongoClient(databaseConnection.ConnectionString).GetDatabase(databaseConnection.DataSource);
                var parsingBson = BsonSerializer.Deserialize<BsonDocument>(query);
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
                                var currentDocument = executingCursor.Current.FirstOrDefault();
                                if (currentDocument != null)
                                {
                                    // Note: Server will decrease the performance for deserializing JSON instead of client
                                    var objsList = executingCursor.Current.Select(a => a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
                                    {
                                        OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                                    })).Select(b =>
                                            JsonConvert.DeserializeObject<dynamic>(b, new BsonConverter())).ToList();
                                    result.Result = objsList.Count > 1 ? objsList : objsList[0];
                                    result.IsSuccess = true;
                                }
                            }
                        }
                        break;
                    case Constants.INSERT_KEY:
                        var mongoInsertCollection = GetCollection(mongoDatabase, parsingBson, Constants.INSERT_KEY);
                        var collectionCreateBson = parsingBson[Constants.INSERT_KEY][0][Constants.DATA_KEY].AsBsonDocument;
                        var insertId = ObjectId.GenerateNewId();
                        if (collectionCreateBson.Any(a => a.Name == "_id"))
                        {
                            collectionCreateBson["_id"] = insertId;
                        }
                        else
                        {
                            collectionCreateBson.Add("_id", insertId);
                        }

                        // Ensure any trouble, remove id field
                        collectionCreateBson.Remove("id");
                        // Getting another fields
                        var anotherFields = parsingBson[Constants.INSERT_KEY][0].AsBsonDocument.Where(a => a.Name != Constants.DATA_KEY);
                        if (anotherFields.Any())
                        {
                            collectionCreateBson.AddRange(anotherFields);
                        }

                        await mongoInsertCollection.InsertOneAsync(collectionCreateBson);
                        // Only return new id
                        result.Result = new ExpandoObject();
                        result.Result.Id = insertId.ToString();
                        break;
                    case Constants.UPDATE_KEY:
                        var mongoUpdateCollection = GetCollection(mongoDatabase, parsingBson, Constants.UPDATE_KEY);
                        var collectionWhereBson = parsingBson[Constants.UPDATE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var updateData = parsingBson[Constants.UPDATE_KEY][0][Constants.DATA_KEY].AsBsonDocument;

                        // Important hack: because mongodb used '_id' is a primary key so that we need to convert id -> _id when update
                        // Remove id
                        updateData.Remove(" _id");
                        updateData.Remove("id");

                        var anotherUpdateFields = parsingBson[Constants.UPDATE_KEY][0].AsBsonDocument.Where(a => a.Name != Constants.DATA_KEY && a.Name != Constants.WHERE_KEY);
                        if (anotherUpdateFields.Any())
                        {
                            updateData.AddRange(anotherUpdateFields);
                        };

                        var set = new BsonDocument("$set", updateData);
                        await mongoUpdateCollection.UpdateOneAsync(collectionWhereBson, set);

                        break;
                    case Constants.DELETE_KEY:
                        var mongoDeleteCollection = GetCollection(mongoDatabase, parsingBson, Constants.DELETE_KEY);
                        var collectionWhereDeleteBson = parsingBson[Constants.DELETE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var deleteResult = await mongoDeleteCollection.DeleteOneAsync(collectionWhereDeleteBson);
                        result.Result = deleteResult.DeletedCount;
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                return ExecuteDynamicResultModel.IsFailed(ex.Message);
            }
        }

        public async Task<ExecuteDynamicResultModel> Execute(List<DatabaseConnection> databaseConnections, DatabaseExecutionChains executionChains, IEnumerable<ExecuteParamModel> parameters)
        {
            var context = new MongoExecutionContext
            {
                Data = JObject.Parse("{}")
            };

            bool isSuccess = false;
            string exception = string.Empty;
            for (int i = 0; i < executionChains?.Steps.Count; i++)
            {
                var step = executionChains.Steps[i];
                var dbConnection = databaseConnections.First(a => a.Id == step.DatabaseConnectionId);
                var stepResult = await Execute(dbConnection, step.ExecuteCommand, parameters, context);
                if (stepResult.IsSuccess)
                {
                    switch (stepResult.ExecutionType)
                    {
                        case StepExecutionType.Query:
                            WriteDataToContext($"step{i.ToString()}", ConvertUtil.SerializeObject(stepResult.Result), context);
                            break;
                        case StepExecutionType.Insert:
                            WriteDataToContext($"step{i.ToString()}", ConvertUtil.SerializeObject(stepResult.Result), context);
                            break;
                        case StepExecutionType.Update:
                            break;
                        case StepExecutionType.Delete:
                            break;
                    }
                }
                else
                {
                    isSuccess = false;
                    exception = stepResult.Error;
                    break;
                }
            }

            return isSuccess ? new ExecuteDynamicResultModel { IsSuccess = isSuccess } : ExecuteDynamicResultModel.IsFailed(exception);
        }

        private async Task<StepExecutionResult> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters, MongoExecutionContext context)
        {
            try
            {
                formattedString = StringUtil.ReplaceDoubleCurlyBraces(formattedString, parameters.Select(a => new Tuple<string, string, bool>(a.Name, a.ReplaceValue, a.RemoveQuotes)));
                formattedString = ReplaceValueWithContext(formattedString, context);
                var result = new StepExecutionResult { IsSuccess = true };
                var query = EliminateRedundantFormat(formattedString);
                var mongoDatabase = new MongoClient(databaseConnection.ConnectionString).GetDatabase(databaseConnection.DataSource);
                var parsingBson = BsonSerializer.Deserialize<BsonDocument>(query);
                var executionGroupType = parsingBson.First().Name;

                switch (executionGroupType)
                {
                    case Constants.QUERY_KEY:
                        result.ExecutionType = StepExecutionType.Query;
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
                                var currentDocument = executingCursor.Current.FirstOrDefault();
                                if (currentDocument != null)
                                {
                                    // Note: Server will decrease the performance for deserializing JSON instead of client
                                    var objsList = executingCursor.Current.Select(a => a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
                                    {
                                        OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                                    })).Select(b =>
                                            JsonConvert.DeserializeObject<dynamic>(b, new BsonConverter())).ToList();
                                    result.Result = objsList.Count > 1 ? objsList : objsList[0];
                                    result.IsSuccess = true;
                                }
                            }
                        }
                        break;
                    case Constants.INSERT_KEY:
                        result.ExecutionType = StepExecutionType.Insert;
                        var mongoInsertCollection = GetCollection(mongoDatabase, parsingBson, Constants.INSERT_KEY);
                        var collectionCreateBson = parsingBson[Constants.INSERT_KEY][0][Constants.DATA_KEY].AsBsonDocument;
                        var insertId = ObjectId.GenerateNewId();
                        if (collectionCreateBson.Any(a => a.Name == "_id"))
                        {
                            collectionCreateBson["_id"] = insertId;
                        }
                        else
                        {
                            collectionCreateBson.Add("_id", insertId);
                        }

                        // Ensure any trouble, remove id field
                        collectionCreateBson.Remove("id");
                        // Getting another fields
                        var anotherFields = parsingBson[Constants.INSERT_KEY][0].AsBsonDocument.Where(a => a.Name != Constants.DATA_KEY);
                        if (anotherFields.Any())
                        {
                            collectionCreateBson.AddRange(anotherFields);
                        }

                        await mongoInsertCollection.InsertOneAsync(collectionCreateBson);
                        // Only return new id
                        result.Result = new ExpandoObject();
                        result.Result.Id = insertId.ToString();
                        break;
                    case Constants.UPDATE_KEY:
                        result.ExecutionType = StepExecutionType.Update;
                        var mongoUpdateCollection = GetCollection(mongoDatabase, parsingBson, Constants.UPDATE_KEY);
                        var collectionWhereBson = parsingBson[Constants.UPDATE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var updateData = parsingBson[Constants.UPDATE_KEY][0][Constants.DATA_KEY].AsBsonDocument;

                        // Important hack: because mongodb used '_id' is a primary key so that we need to convert id -> _id when update
                        // Remove id
                        updateData.Remove(" _id");
                        updateData.Remove("id");

                        var anotherUpdateFields = parsingBson[Constants.UPDATE_KEY][0].AsBsonDocument.Where(a => a.Name != Constants.DATA_KEY && a.Name != Constants.WHERE_KEY);
                        if (anotherUpdateFields.Any())
                        {
                            updateData.AddRange(anotherUpdateFields);
                        };

                        var set = new BsonDocument("$set", updateData);
                        await mongoUpdateCollection.UpdateOneAsync(collectionWhereBson, set);

                        break;
                    case Constants.DELETE_KEY:
                        result.ExecutionType = StepExecutionType.Delete;
                        var mongoDeleteCollection = GetCollection(mongoDatabase, parsingBson, Constants.DELETE_KEY);
                        var collectionWhereDeleteBson = parsingBson[Constants.DELETE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var deleteResult = await mongoDeleteCollection.DeleteOneAsync(collectionWhereDeleteBson);
                        result.Result = deleteResult.DeletedCount;
                        break;
                }
                return result;
            }
            catch(Exception ex)
            {
                return StepExecutionResult.IsFailed(ex.ToString());
            }
            
        }

        private static string ReplaceValueWithContext(string str, MongoExecutionContext context)
        {
            if (context == null || context.Data == null)
            {
                return str;
            }
            var allFields = StringUtil.GetByRegexMatchs(@"{{\$(.*?)}}", str, true);
            if (allFields.Length > 0)
            {
                foreach (var field in allFields)
                {
                    var replacedValue = context.Data.SelectToken(field);
                    switch (replacedValue.Type)
                    {
                        case JTokenType.Object:
                            str = str.Replace("\"{{" + field + "}}\"", replacedValue.ToString(), StringComparison.OrdinalIgnoreCase);
                            break;
                        case JTokenType.Boolean:
                        case JTokenType.Integer:
                            str = str.Replace("\"{{" + field + "}}\"", replacedValue.Value<string>(), StringComparison.OrdinalIgnoreCase);
                            break;
                        default:
                            str = str.Replace("{{" + field + "}}", replacedValue.Value<string>(), StringComparison.OrdinalIgnoreCase);
                            break;
                    }
                }

                return str;
            }
            else
            {
                return str;
            }
        }

        private void WriteDataToContext(string name, string data, MongoExecutionContext context)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (context.Data.Children().Any(a => (a as JProperty).Name == name))
                {
                    var mergeObject = (JObject)context.Data[name];
                    var dataJObject = JObject.Parse(data);
                    mergeObject.Merge(dataJObject, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Merge,
                        MergeNullValueHandling = MergeNullValueHandling.Merge
                    });

                    context.Data.Remove(name);
                    context.Data.Add(name, mergeObject);
                }
                else
                {
                    context.Data.Add(name, JToken.Parse(data));
                }
            }
        }

        private static string EliminateRedundantFormat(string query)
        {
            // Eliminate ObjectId
            var indexObjectId = query.IndexOf("\"ObjectId(");
            while (indexObjectId > 0)
            {
                var closedCurly = query.IndexOf(")", indexObjectId);
                query = query.Remove(closedCurly + 1, 1);
                query = query.Remove(indexObjectId, 1);
                indexObjectId = query.IndexOf("\"ObjectId(");
            }

            // Eliminate ISODate
            var indexISODate = query.IndexOf("\"ISODate(");
            while (indexISODate > 0)
            {
                var closedCurly = query.IndexOf(")", indexISODate);
                query = query.Remove(closedCurly + 1, 1);
                query = query.Remove(indexISODate, 1);
                indexISODate = query.IndexOf("\"ISODate(");
            }

            // Eliminate NumberLong
            var indexNumberLong = query.IndexOf("\"NumberLong(");
            while (indexNumberLong > 0)
            {
                var closedCurly = query.IndexOf(")", indexNumberLong);
                query = query.Remove(closedCurly + 1, 1);
                query = query.Remove(indexNumberLong, 1);
                indexNumberLong = query.IndexOf("\"NumberLong(");
            }

            return query;
        }

        private static IMongoCollection<BsonDocument> GetCollection(IMongoDatabase mongoDatabase, BsonDocument parsingBson, string operatorName)
        {
            var collectionName = parsingBson[operatorName].AsBsonDocument.First().Name;
            return mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }

    class StepExecutionResult
    {
        public StepExecutionType ExecutionType { get; set; }

        public dynamic Result;

        public bool IsSuccess;
        public string Error { get; set; }

        public static StepExecutionResult IsFailed(string errorMessage)
        {
            return new StepExecutionResult { IsSuccess = false, Error = errorMessage };
        }
    }

    class MongoExecutionContext
    {
        public JObject Data { get; set; }
    }

    enum StepExecutionType
    {
        Query,
        Insert,
        Update,
        Delete
    }
}
