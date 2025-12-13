using MySqlConnector;

namespace LetPortal.Portal.Mappers.MySQL
{
    public interface IMySqlMapper
    {
        MySqlDbType GetMySqlDbType(string type);
    }
}
