using System.Data;

namespace LetPortal.Portal.Mappers.SqlServer
{
    public interface ISqlServerMapper
    {
        SqlDbType GetSqlDbType(string type);
    }
}
