using LetPortal.Core.Persistences;
using LetPortal.Portal.Exceptions.Databases;

namespace LetPortal.Tools
{
    public class ToolsOptions
    {
        public StoringConnections MongoStoringConnections { get; set; }
        public StoringConnections PostgreSqlStoringConnections { get; set; }
        public StoringConnections SqlServerStoringConnections { get; set; }
        public StoringConnections MySqlStoringConnections { get; set; }

        public string PatchesFolderPath { get; set; }

        public StoringConnections GetByDatabaseType(ConnectionType type)
        {
            switch (type)
            {
                case ConnectionType.MongoDB:
                    return MongoStoringConnections;
                case ConnectionType.MySQL:
                    return MySqlStoringConnections;
                case ConnectionType.PostgreSQL:
                    return PostgreSqlStoringConnections;
                case ConnectionType.SQLServer:
                    return SqlServerStoringConnections;
                default:
                    throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
            }
        }
    }

    public class StoringConnections
    {
        public DatabaseOptions PortalConnection { get; set; }
        public DatabaseOptions IdentityConnection { get; set; }
        public DatabaseOptions ServiceManagementConnection { get; set; }
    }
}
