using MongoDB.Driver;

namespace LetPortal.Core.Persistences
{
    public class DefaultMongoConnection : IPersistenceConnection<IMongoDatabase>
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public IMongoDatabase GetDatabaseConnection(string databaseName = null)
        {
            var mongoClient = new MongoClient(ConnectionString);

            return mongoClient.GetDatabase(databaseName ?? DatabaseName);
        }

        public void ReloadOptions(DatabaseOptions databaseOptions)
        {
            // Do nothing
        }
    }
}
