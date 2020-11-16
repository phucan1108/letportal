using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Versions.EntitySchemas
{
    public class EntitySchema_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<EntitySchema>("5cbf39888dd09406f03785de");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var databaseConnectionES = new EntitySchema
            {
                Id = "5cbf39888dd09406f03785de",
                DatabaseId = Constants.PortalDatabaseId,
                Name = "databases",
                DisplayName = "Database",
                EntityFields = new System.Collections.Generic.List<EntityField>
                {
                    new EntityField
                    {
                        Name = "id",
                        DisplayName = "Id",
                        FieldType = "string"
                    },
                    new EntityField
                    {
                        Name = "name",
                        DisplayName = "Name",
                        FieldType = "string"
                    },
                    new EntityField
                    {
                        Name = "connectionString",
                        DisplayName = "Connection String",
                        FieldType = "string"
                    },
                    new EntityField
                    {
                        Name = "dataSource",
                        DisplayName = "Datasource",
                        FieldType = "string"
                    },
                    new EntityField
                    {
                        Name = "databaseConnectionType",
                        DisplayName = "Database Connection Type",
                        FieldType = "string"
                    }
                }
            };

            versionContext.InsertData(databaseConnectionES);
            return Task.CompletedTask;
        }
    }
}
