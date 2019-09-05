using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Models;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class ExecuteDatasourceHandler : IRequestHandler<ExecuteDatasourceRequest, ExecutedDataSourceModel>
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IDatasourceRepository _datasourceRepository;

        public ExecuteDatasourceHandler(IDatabaseServiceProvider databaseServiceProvider, IDatasourceRepository datasourceRepository)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _datasourceRepository = datasourceRepository;
        }

        public async Task<ExecutedDataSourceModel> Handle(ExecuteDatasourceRequest request, CancellationToken cancellationToken)
        {
            List<DatasourceModel> datasourceModels = new List<DatasourceModel>();

            var datasource = await _datasourceRepository.GetOneAsync(request.GetQuery().DatasourceId);

            if (datasource.DatasourceType == DatasourceType.Static)
            {
                datasourceModels = ConvertUtil.DeserializeObject<List<DatasourceModel>>(datasource.Query);
            }
            else
            {
                var database = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(datasource.DatabaseId);

                JObject parsingObject = JObject.Parse(datasource.Query);

                var collectionName = parsingObject.Properties().Select(a => a.Name).First();

                var mongoCollection = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource).GetCollection<BsonDocument>(collectionName);

                string collectionQuery = parsingObject[collectionName].ToString(Newtonsoft.Json.Formatting.Indented);

                FilterDefinition<BsonDocument> collectionQueryBson = BsonDocument.Parse(collectionQuery);

                // For ex: the datasource json body should contain the WHERE clause, and returned value will be projected by OutputProjection
                // "apps" : { "id": "a" }
                // OutputProjection: "name=id;value=displayname 
                // Result: { "name": a, "value": "1234" }

                IAggregateFluent<BsonDocument> aggregateFluent = mongoCollection.Aggregate();
                aggregateFluent = aggregateFluent.Match(collectionQueryBson);
                var outputSplitted = datasource.OutputProjection.Split(";");
                BsonDocument projectDoc = new BsonDocument();
                foreach(var split in outputSplitted)
                {
                    var arrays = split.Split("=");
                    projectDoc.Add(new BsonElement(arrays[0], "$" + arrays[1]));
                }
                aggregateFluent = aggregateFluent.Project(projectDoc);
                using (IAsyncCursor<BsonDocument> executingCursor = await aggregateFluent.ToCursorAsync())
                {
                    while (executingCursor.MoveNext())
                    {
                        IEnumerable<BsonDocument> currentDocument = executingCursor.Current;

                        datasourceModels = ConvertUtil.DeserializeObject<List<DatasourceModel>>(currentDocument.ToJson());
                    }
                }
            }

            return new ExecutedDataSourceModel
            {
                DatasourceModels = datasourceModels,
                CanCache = datasource.CanCache
            };
        }
    }

    internal class DatasourceExtract
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
