using System.Data.Common;
using Microsoft.Extensions.Options;

namespace LetPortal.Core.Persistences
{
    public class EFConnection : IPersistenceConnection<DbConnection>
    {
#pragma warning disable CS0649 // Field 'EFConnection._connection' is never assigned to, and will always have its default value null
        private readonly DbConnection _connection;
#pragma warning restore CS0649 // Field 'EFConnection._connection' is never assigned to, and will always have its default value null

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
