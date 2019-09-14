using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public async Task<ExecuteDynamicResultModel> Execute(object database, string formattedString)
        {
            try
            {
                var result = new ExecuteDynamicResultModel { IsSuccess = true };
                var query = EliminateRedundantFormat(formattedString);
                var mongoDatabase = (database as IPersistenceConnection<IMongoDatabase>).GetDatabaseConnection();
                BsonDocument parsingBson = BsonSerializer.Deserialize<BsonDocument>(query);
                var executionGroupType = parsingBson.First().Name;

                switch(executionGroupType)
                {
                    case Constants.QUERY_KEY:
                        var mongoCollection = GetCollection(mongoDatabase, parsingBson, Constants.QUERY_KEY);
                        List<PipelineStageDefinition<BsonDocument, BsonDocument>> aggregatePipes = parsingBson[Constants.QUERY_KEY][0].AsBsonArray.Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();
                        IAggregateFluent<BsonDocument> aggregateFluent = mongoCollection.Aggregate();
                        foreach(PipelineStageDefinition<BsonDocument, BsonDocument> pipe in aggregatePipes)
                        {
                            aggregateFluent = aggregateFluent.AppendStage(pipe);
                        }
                        using(IAsyncCursor<BsonDocument> executingCursor = await aggregateFluent.ToCursorAsync())
                        {
                            while(executingCursor.MoveNext())
                            {
                                BsonDocument currentDocument = executingCursor.Current.FirstOrDefault();
                                if(currentDocument != null)
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
                        BsonDocument collectionCreateBson = parsingBson[Constants.INSERT_KEY][0][Constants.DATA_KEY].AsBsonDocument;
                        var insertId = ObjectId.GenerateNewId();
                        if(collectionCreateBson.Any(a => a.Name == "_id"))
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
                        if(anotherFields.Any())
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
                        BsonDocument collectionWhereBson = parsingBson[Constants.UPDATE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var updateData = parsingBson[Constants.UPDATE_KEY][0][Constants.DATA_KEY].AsBsonDocument;

                        // Important hack: because mongodb used '_id' is a primary key so that we need to convert id -> _id when update
                        // Remove id
                        updateData.Remove(" _id");
                        updateData.Remove("id");

                        var anotherUpdateFields = parsingBson[Constants.UPDATE_KEY][0].AsBsonDocument.Where(a => a.Name != Constants.DATA_KEY && a.Name != Constants.WHERE_KEY);
                        if(anotherUpdateFields.Any())
                        {
                            updateData.AddRange(anotherUpdateFields);
                        };

                        BsonDocument set = new BsonDocument("$set", updateData);
                        await mongoUpdateCollection.UpdateOneAsync(collectionWhereBson, set);

                        break;
                    case Constants.DELETE_KEY:
                        var mongoDeleteCollection = GetCollection(mongoDatabase, parsingBson, Constants.DELETE_KEY);
                        BsonDocument collectionWhereDeleteBson = parsingBson[Constants.DELETE_KEY][0][Constants.WHERE_KEY].AsBsonDocument;
                        var deleteResult = await mongoDeleteCollection.DeleteOneAsync(collectionWhereDeleteBson);
                        result.Result = deleteResult.DeletedCount;
                        break;
                }
                return result;
            }
            catch(Exception ex)
            {
                return ExecuteDynamicResultModel.IsFailed(ex.Message);
            }
        }

        private string EliminateRedundantFormat(string query)
        {
            // Eliminate ObjectId
            var indexObjectId = query.IndexOf("\"ObjectId(");
            while(indexObjectId > 0)
            {
                var closedCurly = query.IndexOf(")\"", indexObjectId);
                query = query.Remove(closedCurly + 1, 1);
                query = query.Remove(indexObjectId, 1);
                indexObjectId = query.IndexOf("\"ObjectId(");
            }

            // Eliminate ISODate
            var indexISODate = query.IndexOf("\"ISODate(");
            while(indexISODate > 0)
            {
                var closedCurly = query.IndexOf(")\"", indexISODate);
                query = query.Remove(closedCurly + 1, 1);
                query = query.Remove(indexISODate, 1);
                indexISODate = query.IndexOf("\"ISODate(");
            }

            return query;
        }

        private IMongoCollection<BsonDocument> GetCollection(IMongoDatabase mongoDatabase, BsonDocument parsingBson, string operatorName)
        {
            var collectionName = parsingBson[operatorName].AsBsonDocument.First().Name;
            return mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }
}
