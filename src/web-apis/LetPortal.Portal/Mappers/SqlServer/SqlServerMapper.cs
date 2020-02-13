using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LetPortal.Core.Persistences;

namespace LetPortal.Portal.Mappers.SqlServer
{
    public class SqlServerMapper : ISqlServerMapper
    {
        private readonly Dictionary<string, string> _mapperDic;

        public SqlServerMapper(MapperOptions mapperOptions)
        {
            _mapperDic = mapperOptions.SQLServer;
        }

        public SqlDbType GetSqlDbType(string type)
        {
            return GetByEnumValue(_mapperDic.First(a => a.Key == type).Value);
        }
        private SqlDbType GetByEnumValue(string enumValue)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType), enumValue);
        }
    }
}
