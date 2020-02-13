using MongoDB.Driver;

namespace LetPortal.Core.Persistences
{
    public class MongoConnection : IPersistenceConnection<IMongoDatabase>
    {
        protected string ConnectionString;

        protected string DatabaseName;

        private IMongoClient mongoClient;

        public MongoConnection(DatabaseOptions databaseOptions)
        {
            ConnectionString = databaseOptions.ConnectionString;
            if (string.IsNullOrEmpty(databaseOptions.Datasource))
            {
                databaseOptions.Datasource = MongoUrl.Create(databaseOptions.ConnectionString).DatabaseName;
            }
            DatabaseName = databaseOptions.Datasource;
        }

        public IMongoDatabase GetDatabaseConnection(string databaseName = null)
        {
            if (mongoClient == null)
            {
                mongoClient = new MongoClient(ConnectionString);
            }

            return mongoClient.GetDatabase(databaseName ?? DatabaseName);
        }

        public void ReloadOptions(DatabaseOptions databaseOptions)
        {
            ConnectionString = databaseOptions.ConnectionString;
            DatabaseName = databaseOptions.Datasource;
        }
    }
}