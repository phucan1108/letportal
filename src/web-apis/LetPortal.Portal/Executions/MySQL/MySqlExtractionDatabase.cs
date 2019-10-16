using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Databases;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExtractionDatabase : IExtractionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString)
        {
            var extractModel = new ExtractingSchemaQueryModel
            {
                ColumnFields = new System.Collections.Generic.List<Models.Shared.ColumnField>()
            };
            using(var mysqlDbConnection = new MySqlConnection(database.ConnectionString))
            {
                mysqlDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                using(var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        foreach(DataColumn dc in dt.Columns)
                        {
                            extractModel.ColumnFields.Add(new Models.Shared.ColumnField
                            {
                                Name = dc.ColumnName,
                                DisplayName = dc.ColumnName,
                                FieldType = GetType(dc.DataType)
                            });
                        }
                    }
                }
            }

            return Task.FromResult(extractModel);
        }

        private string GetType(Type type)
        {
            if(type == typeof(DateTime))
            {
                return "datetime";
            }
            else if(type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(long))
            {
                return "number";
            }
            else if(type == typeof(bool))
            {
                return "boolean";
            }
            else
            {
                return "string";
            }
        }
    }
}
