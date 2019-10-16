using Microsoft.Extensions.Options;
using System.Data.Common;

namespace LetPortal.Core.Persistences
{
    public class EFConnection : IPersistenceConnection<DbConnection>
    {
        private readonly DbConnection _connection;

        public EFConnection(IOptionsMonitor<DatabaseOptions> options)
        {
            ReloadOptions(options.CurrentValue);
        }

        public DbConnection GetDatabaseConnection(string databaseName = null)
        {
            return _connection;
        }

        public void ReloadOptions(DatabaseOptions databaseOptions)
        {
        }
    }
}
