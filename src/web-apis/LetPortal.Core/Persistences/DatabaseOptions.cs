namespace LetPortal.Core.Persistences
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }

        public string Datasource { get; set; }

        public ConnectionType ConnectionType { get; set; }
    }

    public enum ConnectionType
    {
        MongoDB,
        SQLServer
    }
}
