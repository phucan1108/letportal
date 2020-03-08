using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExtractionDatasource : IExtractionDatasource
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public async Task<List<DatasourceModel>> ExtractionDatasource(
            DatabaseConnection databaseConnection,
            string formattedQueryString,
            string outputProjection)
        {

            var datasourceModels = new List<DatasourceModel>();
            var parsingObject = JObject.Parse(formattedQueryString);

            var collectionName = parsingObject.Properties().Select(a => a.Name).First();

            var mongoCollection = new MongoClient(databaseConnection.ConnectionString).GetDatabase(databaseConnection.DataSource).GetCollection<BsonDocument>(collectionName);

            var collectionQuery = parsingObject[collectionName].ToString(Newtonsoft.Json.Formatting.Indented);

            FilterDefinition<BsonDocument> collectionQueryBson = BsonDocument.Parse(collectionQuery);

            // For ex: the datasource json body should contain the WHERE clause, and returned value will be projected by OutputProjection
            // "apps" : { "id": "a" }
            // OutputProjection: "name=id;value=displayname 
            // Result: { "name": a, "value": "1234" }

            var aggregateFluent = mongoCollection.Aggregate();
            aggregateFluent = aggregateFluent.Match(collectionQueryBson);

            var hasProjection = !string.IsNullOrEmpty(outputProjection);
            if (hasProjection)
            {
                var outputSplitted =
                hasProjection ?
                    outputProjection.Split(";") : System.Array.Empty<string>();
                var projectDoc = new BsonDocument();
                foreach (var split in outputSplitted)
                {
                    var arrays = split.Split("=");
                    projectDoc.Add(new BsonElement(arrays[0], "$" + arrays[1]));
                }

                aggregateFluent = aggregateFluent.Project(projectDoc);
            }


            using (var executingCursor = await aggregateFluent.ToCursorAsync())
            {
                while (executingCursor.MoveNext())
                {
                    if (hasProjection)
                    {
                        datasourceModels = executingCursor.Current.Select(a => a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
                        {
                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                        })).Select(b =>
                                JsonConvert.DeserializeObject<DatasourceModel>(b, new BsonConverter())).ToList();
                    }
                    else
                    {
                        var objsList = executingCursor.Current.Select(a => a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
                        {
                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                        })).Select(b =>
                                JsonConvert.DeserializeObject<dynamic>(b, new BsonConverter())).ToList();

                        if (objsList.Count > 0)
                        {
                            foreach (var ob in objsList)
                            {
                                string temp = JsonConvert.SerializeObject(ob);
                                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(temp);
                                var dataModel = new DatasourceModel();
                                if (dic.ContainsKey("id"))
                                {
                                    dataModel.Value = dic["id"];
                                    dataModel.Name = dic.First().Value;
                                }
                                else
                                {
                                    var i = 0;
                                    foreach (var kvp in dic)
                                    {
                                        if (i == 0)
                                        {
                                            dataModel.Name = kvp.Value;
                                        }

                                        if (i == 1)
                                        {
                                            dataModel.Value = kvp.Value;
                                        }

                                        if (i > 1)
                                        {
                                            break;
                                        }
                                        i++;
                                    }
                                }
                                datasourceModels.Add(dataModel);
                            }
                        }
                    }
                }
            }

            return datasourceModels;
        }
    }
}
