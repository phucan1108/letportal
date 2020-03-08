using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;

namespace LetPortal.Portal.Entities.Datasources
{
    [EntityCollection(Name = DatasourceConstants.DatasourceCollection)]
    [Table("datasources")]
    public class Datasource : BackupableEntity
    {
        public DatasourceType DatasourceType { get; set; }

        public bool CanCache { get; set; }

        public string DatabaseId { get; set; }

        public string Query { get; set; }

        public string OutputProjection { get; set; }
    }

    public enum DatasourceType
    {
        Static,
        Database
    }
}
