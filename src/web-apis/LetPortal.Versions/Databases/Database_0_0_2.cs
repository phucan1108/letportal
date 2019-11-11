using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Databases;
using System;

namespace LetPortal.Versions.Databases
{
    public class Database_0_0_2 : IVersion
    {
        public string VersionNumber => "0.0.2";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.ServiceManagementDatabaseId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            DatabaseOptions databaseOptions = versionContext.ServiceManagementOptions as DatabaseOptions;
            var databaseManagement = new DatabaseConnection
            {
                Id = Constants.ServiceManagementDatabaseId,
                ConnectionString = databaseOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databaseOptions.ConnectionType).ToLower(),
                DataSource = databaseOptions.Datasource,
                Name = "Service Management"
            };

            versionContext.InsertData(databaseManagement);
        }
    }
}
