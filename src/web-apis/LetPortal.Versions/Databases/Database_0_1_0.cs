﻿using System;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Databases;

namespace LetPortal.Versions.Databases
{
    public class Database_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.PortalDatabaseId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseOptions = versionContext.DatabaseOptions as DatabaseOptions;
            var databaseManagement = new DatabaseConnection
            {
                Id = Constants.PortalDatabaseId,
                ConnectionString = databaseOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databaseOptions.ConnectionType).ToLower(System.Globalization.CultureInfo.CurrentCulture),
                DataSource = databaseOptions.Datasource,
                Name = "portalDatabase",
                DisplayName = "Portal Database"
            };

            versionContext.InsertData(databaseManagement);
        }
    }

}
