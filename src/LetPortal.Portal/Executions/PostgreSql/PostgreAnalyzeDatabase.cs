using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using Npgsql;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreAnalyzeDatabase : IAnalyzeDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public async Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(DatabaseConnection databaseConnection)
        {
            var postgreInfos = new List<PostgreSchemaInfo>();
            using (var dbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                dbConnection.Open();
                using (var cmd = new NpgsqlCommand("SELECT table_catalog as Catalog, table_schema as Schema, table_name as TableName, column_name as ColumnName, udt_name as Type FROM information_schema.columns WHERE table_schema = 'public'", dbConnection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            postgreInfos.Add(new PostgreSchemaInfo
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

            return (from p in postgreInfos
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

        internal class PostgreSchemaInfo
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
                        return "datetime";
                    case "int16":
                    case "int8":
                    case "int4":
                        return "number";
                    case "bool":
                        return "boolean";
                    case "text":
                    default:
                        return "string";
                }
            }
        }
    }
}
