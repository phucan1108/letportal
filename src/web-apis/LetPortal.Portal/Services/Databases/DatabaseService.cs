using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models;
using LetPortal.Portal.Repositories.Databases;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LetPortal.Portal.Services.Databases
{
    public class DatabaseService : IDatabaseService
    {
        private const string QUERY_KEY = "$query";
        private const string INSERT_KEY = "$insert";
        private const string UPDATE_KEY = "$update";
        private const string DELETE_KEY = "$delete";
        private const string DATA_KEY = "$data";
        private const string WHERE_KEY = "$where";

        private IDatabaseRepository _databaseRepository;

        public DatabaseService(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDynamic(string databaseId, string formattedString)
        {
            try
            {
                var result = new ExecuteDynamicResultModel { IsSuccess = true };
                var query = EliminateRedundantFormat(formattedString);
                var database = await _databaseRepository.GetOneAsync(databaseId);
                var mongoDatabase = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource);
                BsonDocument parsingBson = BsonSerializer.Deserialize<BsonDocument>(query);
                var executionGroupType = parsingBson.First().Name;

                switch(executionGroupType)
                {
                    case QUERY_KEY:
                        var mongoCollection = GetCollection(mongoDatabase, parsingBson, QUERY_KEY);
                        List<PipelineStageDefinition<BsonDocument, BsonDocument>> aggregatePipes = parsingBson[QUERY_KEY][0].AsBsonArray.Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();
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
                    case INSERT_KEY:
                        var mongoInsertCollection = GetCollection(mongoDatabase, parsingBson, INSERT_KEY);
                        BsonDocument collectionCreateBson = parsingBson[INSERT_KEY][0][DATA_KEY].AsBsonDocument;
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
                        var anotherFields = parsingBson[INSERT_KEY][0].AsBsonDocument.Where(a => a.Name != DATA_KEY);
                        if(anotherFields.Any())
                        {
                            collectionCreateBson.AddRange(anotherFields);
                        }

                        await mongoInsertCollection.InsertOneAsync(collectionCreateBson);
                        // Only return new id
                        result.Result = new { id = insertId };
                        break;
                    case UPDATE_KEY:
                        var mongoUpdateCollection = GetCollection(mongoDatabase, parsingBson, UPDATE_KEY);
                        BsonDocument collectionWhereBson = parsingBson[UPDATE_KEY][0][WHERE_KEY].AsBsonDocument;
                        var updateData = parsingBson[UPDATE_KEY][0][DATA_KEY].AsBsonDocument;

                        // Important hack: because mongodb used '_id' is a primary key so that we need to convert id -> _id when update
                        // Remove id
                        updateData.Remove(" _id");
                        updateData.Remove("id");

                        var anotherUpdateFields = parsingBson[UPDATE_KEY][0].AsBsonDocument.Where(a => a.Name != DATA_KEY && a.Name != WHERE_KEY);
                        if(anotherUpdateFields.Any())
                        {
                            updateData.AddRange(anotherUpdateFields);
                        };

                        BsonDocument set = new BsonDocument("$set", updateData);
                        await mongoUpdateCollection.UpdateOneAsync(collectionWhereBson, set);

                        break;
                    case DELETE_KEY:
                        var mongoDeleteCollection = GetCollection(mongoDatabase, parsingBson, DELETE_KEY);                        
                        BsonDocument collectionWhereDeleteBson = parsingBson[DELETE_KEY][0][WHERE_KEY].AsBsonDocument;
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
