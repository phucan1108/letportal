using LetPortal.Core.Persistences;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetPortal.Portal.Mappers.PostgreSql
{
    public class PostgreSqlMapper : IPostgreSqlMapper
    {
        private readonly Dictionary<string, string> _mapperDic;

        public PostgreSqlMapper(MapperOptions mapperOptions)
        {
            _mapperDic = mapperOptions.PostgreSQL;
        }

        public NpgsqlDbType GetNpgsqlDbType(string type)
        {
            return GetByEnumValue(_mapperDic.First(a => a.Key == type).Value);
        }

        private NpgsqlDbType GetByEnumValue(string enumValue)
        {
            return (NpgsqlDbType)Enum.Parse(typeof(NpgsqlDbType), enumValue);
        }
    }
}
