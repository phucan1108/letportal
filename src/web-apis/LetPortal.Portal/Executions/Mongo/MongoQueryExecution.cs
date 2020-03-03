using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoQueryExecution : IMongoQueryExecution
    {
        private readonly IOptionsMonitor<MongoOptions> _mongoOptions;

        public MongoQueryExecution(IOptionsMonitor<MongoOptions> options)
        {
            _mongoOptions = options;
        }

        public async Task<dynamic> ExecuteAsync(
            IMongoDatabase mongoDatabase,
            string formattedString,
            string mappingProjection,
            IEnumerable<ExecuteParamModel> parameters,
            List<PipelineStageDefinition<BsonDocument, BsonDocument>> filterStages = null)
        {
            formattedString = StringUtil.ReplaceDoubleCurlyBraces(formattedString, parameters?.Select(a => new Tuple<string, string, bool>(a.Name, a.ReplaceValue, a.RemoveQuotes)));
            var query = _mongoOptions.CurrentValue.EliminateDoubleQuotes(formattedString);
            var parsingBson = BsonSerializer.Deserialize<BsonDocument>(query);
            // We are supporting many sample queries:
            // 1) One collection:
            // {
            //   "$query": {
            //      "services": [
            //          { 
            //              "$match" : {
            //                  "$eq" : {
            //                      "id" : "ObjectId('aaa')"
            //                  }
            //              }
            //          }]
            //  }
            //}
            // 2) Two and more collections with union:
            // {
            //   "$query": {
            //      "$union": [
            //          {"services": [
            //              { 
            //                  "$match" : {
            //                      "$eq" : {
            //                          "id" : "ObjectId('aaa')"
            //                      }
            //                  }
            //              }]},
            //          {"services": [
            //              { 
            //                  "$match" : {
            //                      "$eq" : {
            //                          "id" : "ObjectId('aaa')"
            //                      }
            //                  }
            //              }]}
            //      ]
            //  }
            //}
            // 3) TBD - Two or more collections with join:
            // {
            //   "$query": {
            //      "$join": [
            //          { 
            //              "col2.serviceId" : "col1.id",
            //              "col3.serviceId" : "col1.id"
            //          },
            //          {"services": [
            //              { 
            //                  "$match" : {
            //                      "$eq" : {
            //                          "id" : "ObjectId('aaa')"
            //                      }
            //                  }
            //              }]},
            //          {"services": [
            //              { 
            //                  "$match" : {
            //                      "$eq" : {
            //                          "id" : "ObjectId('aaa')"
            //                      }
            //                  }
            //              }]}
            //      ]
            //  }
            //}
            var hasProjection = CheckMappingProjection(mappingProjection);
            var projection = hasProjection ? ConvertMappingProjection(mappingProjection) : null;
            // 1) Check $union/$join or normal 
            if (parsingBson[Constants.QUERY_KEY].AsBsonDocument.Elements.First().Name == Constants.UNION_KEY)
            {
                var arrayUnions = parsingBson[Constants.QUERY_KEY][Constants.UNION_KEY];
                if (arrayUnions != null && arrayUnions.IsBsonArray)
                {
                    var arrayUnionCollections = arrayUnions.AsBsonArray;
                    var fluentCollectionsList = new List<IAggregateFluent<BsonDocument>>();

                    foreach (BsonDocument collection in arrayUnionCollections)
                    {
                        var mongoCollection = mongoDatabase?.GetCollection<BsonDocument>(collection.First().Name);
                        var aggregatePipes = collection.First().Value.AsBsonArray.Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();
                        var aggregateFluent = mongoCollection.Aggregate();

                        var indexMatchStage = -1;
                        var lastMatchStage = aggregatePipes.LastOrDefault(a => a.OperatorName == "$match");
                        if (lastMatchStage != null)
                        {
                            indexMatchStage = aggregatePipes.LastIndexOf(lastMatchStage);
                        }
                        if (filterStages != null)
                        {
                            var needToAppendByIndex = indexMatchStage > -1;
                            foreach (var pipe in filterStages)
                            {
                                if (needToAppendByIndex)
                                {
                                    aggregatePipes.Insert(indexMatchStage + 1, pipe);
                                    indexMatchStage++;
                                }
                                else
                                {
                                    aggregatePipes.Add(pipe);
                                }
                            }
                        }
                        foreach (var pipe in aggregatePipes)
                        {
                            aggregateFluent = aggregateFluent.AppendStage(pipe);
                        }

                        if (hasProjection)
                        {
                            aggregateFluent = aggregateFluent.AppendStage((PipelineStageDefinition<BsonDocument, BsonDocument>)projection);
                        }
                        fluentCollectionsList.Add(aggregateFluent);
                    }
                    var allDynamics = new List<dynamic>();
                    var parallelTasks = new List<Task>();
                    foreach (var collect in fluentCollectionsList)
                    {
                        using (var executingCursor = collect.ToCursor())
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

                                    allDynamics.AddRange(objsList);
                                }
                            }
                        }
                    }
                    return allDynamics;
                }
            }
            else
            {
                _ = mongoDatabase ?? throw new NullReferenceException(nameof(mongoDatabase));
                var mongoCollection = GetCollection(mongoDatabase, parsingBson, Constants.QUERY_KEY);
                var aggregatePipes = parsingBson[Constants.QUERY_KEY][0].AsBsonArray.Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();
                var aggregateFluent = mongoCollection.Aggregate();
                var indexMatchStage = -1;
                var lastMatchStage = aggregatePipes.LastOrDefault(a => a.OperatorName == "$match");
                if (lastMatchStage != null)
                {
                    indexMatchStage = aggregatePipes.LastIndexOf(lastMatchStage);
                }
                if (filterStages != null)
                {
                    var needToAppendByIndex = indexMatchStage > -1;
                    foreach (var pipe in filterStages)
                    {
                        if (needToAppendByIndex)
                        {
                            aggregatePipes.Insert(indexMatchStage + 1, pipe);
                            indexMatchStage++;
                        }
                        else
                        {
                            aggregatePipes.Add(pipe);
                        }
                    }
                }
                foreach (var pipe in aggregatePipes)
                {
                    aggregateFluent = aggregateFluent.AppendStage(pipe);
                }

                if (hasProjection)
                {
                    aggregateFluent = aggregateFluent.AppendStage((PipelineStageDefinition<BsonDocument, BsonDocument>)projection);
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
                            return objsList;
                        }
                    }
                }
            }

            return null;
        }

        private bool CheckMappingProjection(string mappingProjection)
        {
            if (string.IsNullOrEmpty(mappingProjection))
            {
                return false;
            }

            return true;
        }

        private BsonDocument ConvertMappingProjection(string mappingProjection)
        {
            var projectDoc = new BsonDocument();

            var splitted = mappingProjection.Split(";");
            foreach (var split in splitted)
            {
                if (string.IsNullOrEmpty(split))
                {
                    continue;
                }
                var sub = split.Split("=");
                projectDoc.Add(new BsonElement(sub[0], "$" + sub[1]));
            }
            var projection = new BsonDocument
            {
                { "$project", projectDoc }
            };

            return projection;
        }

        private string EliminateRedundantFormat(string query)
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

        private IMongoCollection<BsonDocument> GetCollection(IMongoDatabase mongoDatabase, BsonDocument parsingBson, string operatorName)
        {
            var collectionName = parsingBson[operatorName].AsBsonDocument.First().Name;
            return mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }
}
