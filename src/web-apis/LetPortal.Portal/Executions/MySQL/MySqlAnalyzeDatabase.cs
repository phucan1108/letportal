using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlAnalyzeDatabase : IAnalyzeDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public async Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(DatabaseConnection databaseConnection)
        {
            var mysqlInfos = new List<MySqlSchemaInfo>();
            using (var dbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                dbConnection.Open();
                using (var cmd = new MySqlCommand("SELECT table_catalog as 'Catalog', table_schema as 'Schema', table_name as TableName, column_name as ColumnName, data_type as Type FROM information_schema.columns Where table_schema='letportal'", dbConnection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            mysqlInfos.Add(new MySqlSchemaInfo
                            {
                                Catalog = reader.GetString(0),
                                Schema = reader.GetString(1),
                                TableName = reader.GetString(2),
                                ColumnName = reader.GetString(3),
                                Type = reader.GetString(4)
                            });
                        }
                    }
                }
                dbConnection.Close();
            }

            return (from p in mysqlInfos
                    group p by p.TableName into tableGroup
                    select new EntitySchema
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        DatabaseId = databaseConnection.Id,
                        DisplayName = tableGroup.Key,
                        Name = tableGroup.Key,
                        TimeSpan = DateTime.UtcNow.Ticks,
                        EntityFields = (from col in tableGroup
                                        select new EntityField
                                        {
                                            Name = col.ColumnName,
                                            DisplayName = col.ColumnName,
                                            FieldType = col.GetDataType()
                                        }).ToList()
                    });
        }

        internal class MySqlSchemaInfo
        {
            public string Catalog { get; set; }
            public string Schema { get; set; }

            public string TableName { get; set; }

            public string ColumnName { get; set; }

            public string Type { get; set; }

            public string GetDataType()
            {
                switch (Type)
                {
                    case "timestamp":
                    case "datetime2":
                    case "date":
                    case "datetime":
                        return "datetime";
                    case "int":
                    case "tinyint":
                    case "mediumint":
                    case "bigint":
                    case "float":
                    case "decimal":
                    case "money":
                        return "number";
                    case "bit":
                    case "bool":
                    case "boolean":
                        return "boolean";
                    case "text":
                    case "nvarchar":
                    default:
                        return "string";
                }
            }
        }
    }
}
