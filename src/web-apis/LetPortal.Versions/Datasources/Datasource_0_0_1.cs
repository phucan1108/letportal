using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Datasources;
using System;

namespace LetPortal.Versions.Datasources
{
    public class Datasource_0_0_1 : IVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Datasource>("5c289b1d3be42159cc2f0236");
            versionContext.DeleteData<Datasource>("5c06a15e4cc9a850bca44488");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseConnectionType = new Datasource
            {
                Id = "5c289b1d3be42159cc2f0236",
                CanCache = true,
                Name = "Database Connection Type",
                DatabaseId = "",
                DatasourceType = DatasourceType.Static,
                Query = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}]"                
            };

            var appConnectionType = new Datasource
            {
                Id = "5c06a15e4cc9a850bca44488",
                CanCache = true,
                Name = "Apps List",
                DatabaseId = Constants.CoreDatabaseId,
                DatasourceType = DatasourceType.Database,
                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? "{\"apps\":{}}" : "SELECT displayName as name, id as value From apps",
                OutputProjection = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? "name=displayName;value=_id" : null
            };

            versionContext.InsertData(databaseConnectionType);
            versionContext.InsertData(appConnectionType);
        }
    }
}
