using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;
using System.Data.SqlClient;

namespace LetPortal.Core.Persistences
{
    public class EFConnection : IPersistenceConnection<DbConnection>
    {
        private DbConnection _connection;

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
            if(databaseOptions.ConnectionType == ConnectionType.SQLServer)
            {
                _connection = new SqlConnection(databaseOptions.ConnectionString);
            }
            else if(databaseOptions.ConnectionType == ConnectionType.PostgreSQL)
            {
                _connection = new NpgsqlConnection(databaseOptions.ConnectionString);
            }
        }
    }
}
