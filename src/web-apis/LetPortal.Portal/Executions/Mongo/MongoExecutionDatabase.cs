using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        private readonly IOptionsMonitor<MongoOptions> _mongoOptions;

        public MongoExecutionDatabase(IOptionsMonitor<MongoOptions> mongoOptions)
        {
            _mongoOptions = mongoOptions;
        }

        public async Task<ExecuteDynamicResultModel> Execute(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            IEnumerable<ExecuteParamModel> parameters)
        {
            try
            {                   
                formattedString = StringUtil.ReplaceDoubleCurlyBraces(formattedString, parameters.Select(a => new Tuple<string, string, bool>(a.Name, a.ReplaceValue, a.RemoveQuotes)));
                var result = new ExecuteDynamicResultModel { IsSuccess = true };
                var query = _mongoOptions.CurrentValue.EliminateDoubleQuotes(formattedString);
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


        public async Task<StepExecutionResult> ExecuteStep(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters, ExecutionDynamicContext context)
        {
            if (databaseConnection == null)
            {
                throw new ArgumentNullException(nameof(databaseConnection));
            }
            return await ExecuteWithContext(databaseConnection, formattedString, parameters);
        }

        private async Task<StepExecutionResult> ExecuteWithContext(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            try
            {
                formattedString = StringUtil.ReplaceDoubleCurlyBraces(formattedString, parameters.Select(a => new Tuple<string, string, bool>(a.Name, a.ReplaceValue, a.RemoveQuotes)));
                var result = new StepExecutionResult { IsSuccess = true };
                var query = _mongoOptions.CurrentValue.EliminateDoubleQuotes(formattedString);
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                return StepExecutionResult.IsFailed(ex.ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types

        }

        private static IMongoCollection<BsonDocument> GetCollection(IMongoDatabase mongoDatabase, BsonDocument parsingBson, string operatorName)
        {
            var collectionName = parsingBson[operatorName].AsBsonDocument.First().Name;
            return mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }
}
