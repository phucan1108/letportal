using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Exceptions.Databases;

namespace LetPortal.Tools
{
    public class ToolsOptions
    {
        public IEnumerable<string> RequiredPlugins { get; set; }
        public IEnumerable<string> RequiredPatchs { get; set; }
        public StoringConnections MongoStoringConnections { get; set; }
        public StoringConnections PostgreSqlStoringConnections { get; set; }
        public StoringConnections SqlServerStoringConnections { get; set; }
        public StoringConnections MySqlStoringConnections { get; set; }

        public string PatchesFolderPath { get; set; }

        public StoringConnections GetByDatabaseType(ConnectionType type)
        {
            return type switch
            {
                ConnectionType.MongoDB => MongoStoringConnections,
                ConnectionType.MySQL => MySqlStoringConnections,
                ConnectionType.PostgreSQL => PostgreSqlStoringConnections,
                ConnectionType.SQLServer => SqlServerStoringConnections,
                _ => throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType),
            };
        }
    }

    public class StoringConnections
    {
        public DatabaseOptions PortalConnection { get; set; }
        public DatabaseOptions IdentityConnection { get; set; }
        public DatabaseOptions ServiceManagementConnection { get; set; }
    }
}
