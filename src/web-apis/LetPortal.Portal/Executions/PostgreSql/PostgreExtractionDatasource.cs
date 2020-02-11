using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using Npgsql;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreExtractionDatasource : IExtractionDatasource
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<List<DatasourceModel>> ExtractionDatasource(DatabaseConnection databaseConnection, string formattedQueryString, string outputProjection)
        {
            var datasources = new List<DatasourceModel>();
            using (var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                using (var cmd = new NpgsqlCommand(formattedQueryString, postgreDbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        var hasNameAndValueCol = dt.Columns.Contains("Name") && dt.Columns.Contains("Value");
                        if (!hasNameAndValueCol)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                datasources.Add(new DatasourceModel
                                {
                                    Name = dr[1].ToString(),
                                    Value = dr[0].ToString()
                                });
                            }
                        }
                        else
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                datasources.Add(new DatasourceModel
                                {
                                    Name = dr["Name"].ToString(),
                                    Value = dr["Value"].ToString()
                                });
                            }
                        }
                        dt.Dispose();
                    }
                }
            }
            return Task.FromResult(datasources);
        }
    }
}
