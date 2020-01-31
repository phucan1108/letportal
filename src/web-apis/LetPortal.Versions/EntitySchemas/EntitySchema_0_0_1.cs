using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Versions.EntitySchemas
{
    public class EntitySchema_0_0_1 : IPortalVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<EntitySchema>("5cbf39888dd09406f03785de");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseConnectionES = new EntitySchema
            {
                Id = "5cbf39888dd09406f03785de",
                DatabaseId = Constants.CoreDatabaseId,
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
        }
    }
}
