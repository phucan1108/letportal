using LetPortal.Core.Persistences;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetPortal.Portal.Mappers.MySQL
{
    public class MySqlMapper : IMySqlMapper
    {
        private readonly IDictionary<string, string> _mapperDic;

        public MySqlMapper(MapperOptions mapperOptions)
        {
            _mapperDic = mapperOptions.MySQL;
        }

        public MySqlDbType GetMySqlDbType(string type)
        {
            return GetByEnumValue(_mapperDic.First(a => a.Key == type).Value);
        }

        private MySqlDbType GetByEnumValue(string enumValue)
        {
            return (MySqlDbType)Enum.Parse(typeof(MySqlDbType), enumValue);
        }
    }
}
