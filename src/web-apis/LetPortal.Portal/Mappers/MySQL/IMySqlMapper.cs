using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Mappers.MySQL
{
    public interface IMySqlMapper
    {
        MySqlDbType GetMySqlDbType(string type);
    }
}
