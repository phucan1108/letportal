using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExtractionDatasource : IExtractionDatasource
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public Task<List<DatasourceModel>> ExtractionDatasource(DatabaseConnection databaseConnection, string formattedQueryString, string outputProjection)
        {
            var datasources = new List<DatasourceModel>();
            using(var mySqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mySqlDbConnection.Open();
                using(var cmd = new MySqlCommand(formattedQueryString, mySqlDbConnection))
                {
                    using(var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        bool hasNameAndValueCol = dt.Columns.Contains("Name") && dt.Columns.Contains("Value");
                        if(!hasNameAndValueCol)
                        {
                            foreach(DataRow dr in dt.Rows)
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
                            foreach(DataRow dr in dt.Rows)
                            {
                                datasources.Add(new DatasourceModel
                                {
                                    Name = dr["Name"].ToString(),
                                    Value = dr["Value"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            return Task.FromResult(datasources);
        }
    }
}
