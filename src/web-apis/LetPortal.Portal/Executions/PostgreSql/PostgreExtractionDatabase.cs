using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Databases;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreExtractionDatabase : IExtractionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            if(parameters != null)
            {
                foreach(var param in parameters)
                {
                    if(param.RemoveQuotes)
                    {
                        formattedString = formattedString.Replace("'{{" + param.Name + "}}'", param.ReplaceValue);
                    }
                    else
                    {
                        formattedString = formattedString.Replace("{{" + param.Name + "}}", param.ReplaceValue);
                    }

                }
            }

            var extractModel = new ExtractingSchemaQueryModel
            {
                ColumnFields = new System.Collections.Generic.List<Models.Shared.ColumnField>()
            };
            using(var postgreDbConnection = new NpgsqlConnection(database.ConnectionString))
            {
                postgreDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                using(var command = new NpgsqlCommand(formattedString, postgreDbConnection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        using(DataTable dt = new DataTable())
                        {
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
