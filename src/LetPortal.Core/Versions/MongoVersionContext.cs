using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using MongoDB.Driver;

namespace LetPortal.Core.Versions
{
    public class MongoVersionContext : IVersionContext
    {
        private readonly IMongoClient mongoClient;

        private readonly IMongoDatabase mongoDatabase;

        public MongoVersionContext(DatabaseOptions databaseOptions)
        {
            if (mongoClient == null)
            {
                mongoClient = new MongoClient(databaseOptions.ConnectionString);
            }

            mongoDatabase = mongoClient.GetDatabase(databaseOptions.Datasource);
        }

        public ConnectionType ConnectionType { get; set; } = ConnectionType.MongoDB;
        public object DatabaseOptions { get; set; }
        public object ServiceManagementOptions { get; set; }
        public object IdentityDbOptions { get; set; }

        public object PortalDatabaseOptions { get; set; }

        public void BulkDeleteData<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            var entityCollection = GetMongoCollection<T>();
            entityCollection.DeleteMany(expression);
        }

        public void BulkInsertData<T>(T[] entities) where T : Entity
        {
            foreach (var entity in entities)
            {
                entity.Check();
            }
            var entityCollection = GetMongoCollection<T>();
            entityCollection.InsertMany(entities);
        }

        public void DeleteData<T>(string id) where T : Entity
        {
            var entityCollection = GetMongoCollection<T>();
            entityCollection.DeleteOne(a => a.Id == id);
        }

        public IQueryable<T> GetQueryable<T>() where T : Entity
        {
            return GetMongoCollection<T>().AsQueryable();
        }

        public void DropAll<T>() where T : Entity
        {
            mongoDatabase.DropCollection(GetEntityName(typeof(T)));
        }

        public void ExecuteRaw(string rawCommand)
        {
            // Do nothing
        }

        public void InsertData<T>(T entity) where T : Entity
        {
            entity.Check();
            var entityCollection = GetMongoCollection<T>();
            entityCollection.InsertOne(entity);
        }

        public void SaveChange()
        {
            // Do nothing
        }

        public void UpdateData<T>(string id, T entity) where T : Entity
        {
            entity.Check();
            var entityCollection = GetMongoCollection<T>();
            entityCollection.FindOneAndReplace(a => a.Id == id, entity);
        }

        private string GetEntityName(Type type)
        {
            return type.GetCustomAttribute<EntityCollectionAttribute>().Name;
        }

        private IMongoCollection<T> GetMongoCollection<T>()
        {
            return mongoDatabase.GetCollection<T>(GetEntityName(typeof(T)));
        }
    }
}
