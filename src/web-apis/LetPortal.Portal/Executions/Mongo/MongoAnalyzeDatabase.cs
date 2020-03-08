using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoAnalyzeDatabase : IAnalyzeDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(DatabaseConnection databaseConnection)
        {
            var currentDatabaseConnection = new MongoClient(databaseConnection.ConnectionString).GetDatabase(databaseConnection.DataSource);

            var cursorCollectionNames = currentDatabaseConnection.ListCollectionNames();

            var analyzedEntitySchemas = new List<EntitySchema>();

            while (cursorCollectionNames.MoveNext())
            {
                var currentCollectionNamesList = cursorCollectionNames.Current;

                foreach (var collectionName in currentCollectionNamesList)
                {
                    var analyzingCollection = currentDatabaseConnection.GetCollection<BsonDocument>(collectionName);

                    FieldDefinition<BsonDocument, string> field = "$natural";
                    var sort = Builders<BsonDocument>.Sort.Descending(field);                    
                    var firstElem = analyzingCollection.Find(Builders<BsonDocument>.Filter.Empty).Sort(sort).Limit(1).FirstOrDefault();

                    if (firstElem != null)
                    {
                        analyzedEntitySchemas.Add(AnalyzeOneBsonDocument(collectionName, firstElem));
                    }
                }
            }

            return Task.FromResult(analyzedEntitySchemas.AsEnumerable());
        }

        private EntitySchema AnalyzeOneBsonDocument(string entityName, BsonDocument bsonDocument)
        {
            var entitySchema = new EntitySchema { Name = entityName, DisplayName = entityName };

            foreach (var bsonElem in bsonDocument)
            {
                var entityField = new EntityField
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
