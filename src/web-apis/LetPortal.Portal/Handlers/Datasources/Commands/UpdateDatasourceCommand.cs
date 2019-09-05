using LetPortal.Portal.Entities.Datasources;

namespace LetPortal.Portal.Handlers.Datasources.Commands
{
    public class UpdateDatasourceCommand
    {
        public string DatasourceId { get; set; }

        public Datasource Datasource { get; set; }
    }
}
