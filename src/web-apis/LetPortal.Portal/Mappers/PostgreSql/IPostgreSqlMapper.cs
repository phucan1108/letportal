using NpgsqlTypes;

namespace LetPortal.Portal.Mappers.PostgreSql
{
    public interface IPostgreSqlMapper
    {
        NpgsqlDbType GetNpgsqlDbType(string type);
    }
}
