using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExtractionDatasource : IExtractionDatasource
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public Task<List<DatasourceModel>> ExtractionDatasource(DatabaseConnection databaseConnection, string formattedQueryString, string outputProjection)
        {
            var datasources = new List<DatasourceModel>();
            using (var sqlDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                sqlDbConnection.Open();
                using (var cmd = new SqlCommand(formattedQueryString, sqlDbConnection))
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
