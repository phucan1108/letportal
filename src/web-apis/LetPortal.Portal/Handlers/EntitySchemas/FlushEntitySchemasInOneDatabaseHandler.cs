using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class FlushEntitySchemasInOneDatabaseHandler : IRequestHandler<FlushEntitySchemasInOneDatabaseRequest, List<EntitySchema>>
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public FlushEntitySchemasInOneDatabaseHandler(IDatabaseServiceProvider databaseServiceProvider, IEntitySchemaRepository entitySchemaRepository)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _entitySchemaRepository = entitySchemaRepository;
        }

        public async Task<List<EntitySchema>> Handle(FlushEntitySchemasInOneDatabaseRequest request, CancellationToken cancellationToken)
        {
            string databaseId = request.GetCommand().DatabaseId;

            DatabaseConnection database = _databaseServiceProvider.GetOneDatabaseConnectionAsync(databaseId).Result;
            IMongoDatabase currentDatabaseConnection = new MongoClient(database.ConnectionString).GetDatabase(database.DataSource);

            IAsyncCursor<string> cursorCollectionNames = currentDatabaseConnection.ListCollectionNames();

            List<EntitySchema> analyzedEntitySchemas = new List<EntitySchema>();

            while (cursorCollectionNames.MoveNext())
            {
                IEnumerable<string> currentCollectionNamesList = cursorCollectionNames.Current;

                foreach (string collectionName in currentCollectionNamesList)
                {
                    IMongoCollection<BsonDocument> analyzingCollection = currentDatabaseConnection.GetCollection<BsonDocument>(collectionName);

                    BsonDocument firstElem = analyzingCollection.AsQueryable().FirstOrDefault();

                    if (firstElem != null)
                    {
                        EntitySchema entitySchema = AnalyzeOneBsonDocument(collectionName, firstElem);

                        entitySchema.DatabaseId = databaseId;

                        analyzedEntitySchemas.Add(entitySchema);
                    }
                }
            }

            foreach (EntitySchema entitySchema in analyzedEntitySchemas)
            {
                bool isExisted = _entitySchemaRepository.GetAsQueryable().Any(a => a.Name == entitySchema.Name);
                if ((isExisted && request.GetCommand().KeptSameName) == false)
                {
                    entitySchema.Id = DataUtil.GenerateUniqueId();
                    await _entitySchemaRepository.AddAsync(entitySchema);
                }
            }

            return analyzedEntitySchemas;
        }

        private EntitySchema AnalyzeOneBsonDocument(string entityName, BsonDocument bsonDocument)
        {
            EntitySchema entitySchema = new EntitySchema { Name = entityName, DisplayName = entityName };

            foreach (BsonElement bsonElem in bsonDocument)
            {
                EntityField entityField = new EntityField
                {
                    Name = bsonElem.Name,
                    DisplayName = bsonElem.Name,
                    FieldType = GetTypeByBsonDocument(bsonElem.Value)
                };

                entitySchema.EntityFields.Add(entityField);
            }

            return entitySchema;
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
