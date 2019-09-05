using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Providers.Databases;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class FetchAllEntitiesFromDatabaseHandler : RequestHandler<FetchAllEntitiesFromDatabaseRequest, List<EntitySchema>>
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        public FetchAllEntitiesFromDatabaseHandler(IDatabaseServiceProvider databaseServiceProvider)
        {
            _databaseServiceProvider = databaseServiceProvider;
        }

        protected override List<EntitySchema> Handle(FetchAllEntitiesFromDatabaseRequest request)
        {
            DatabaseConnection database = _databaseServiceProvider.GetOneDatabaseConnectionAsync(request.GetQuery().DatabaseId).Result;

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
                        analyzedEntitySchemas.Add(AnalyzeOneBsonDocument(collectionName, firstElem));
                    }
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
