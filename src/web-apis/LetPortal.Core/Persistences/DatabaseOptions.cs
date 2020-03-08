using System.Collections.Generic;

namespace LetPortal.Core.Persistences
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }

        public string Datasource { get; set; }

        public ConnectionType ConnectionType { get; set; }
    }

    public class MapperOptions
    {
        public Dictionary<string, string> MongoDB { get; set; }

        public Dictionary<string, string> SQLServer { get; set; }

        public Dictionary<string, string> MySQL { get; set; }

        public Dictionary<string, string> PostgreSQL { get; set; }
    }

    public enum ConnectionType
    {
        MongoDB,
        SQLServer,
        PostgreSQL,
        MySQL
    }
}
