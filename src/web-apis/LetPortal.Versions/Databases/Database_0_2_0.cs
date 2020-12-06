using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Databases;

namespace LetPortal.Versions.Databases
{
    public class Database_0_2_0 : IPortalVersion
    {
        public string VersionNumber => "0.2.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.ServiceManagementDatabaseId);
            versionContext.DeleteData<DatabaseConnection>(Constants.IdentityDatabaseId);
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var databaseSMOptions = versionContext.ServiceManagementOptions as DatabaseOptions;
            var databseIdOptions = versionContext.IdentityDbOptions as DatabaseOptions;
            var smDatabase = new DatabaseConnection
            {
                Id = Constants.ServiceManagementDatabaseId,
                ConnectionString = databaseSMOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databaseSMOptions.ConnectionType).ToLower(System.Globalization.CultureInfo.CurrentCulture),
                DataSource = databaseSMOptions.Datasource,
                Name = "serviceManagement",
                DisplayName = "Service Management"
            };

            var identityDatabase = new DatabaseConnection
            {
                Id = Constants.IdentityDatabaseId,
                ConnectionString = databseIdOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databseIdOptions.ConnectionType).ToLower(System.Globalization.CultureInfo.CurrentCulture),
                DataSource = databseIdOptions.Datasource,
                Name = "identityDatabase",
                DisplayName = "Identity Database"
            };

            versionContext.InsertData(smDatabase);
            versionContext.InsertData(identityDatabase);
            return Task.CompletedTask;
        }
    }
}
