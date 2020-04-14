using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;

namespace LetPortal.Portal.Entities.Databases
{
    [EntityCollection(Name = DatabaseConstants.DatabaseCollection)]
    [Table("databases")]
    public class DatabaseConnection : BackupableEntity
    {
        [StringLength(500)]
        public string ConnectionString { get; set; }

        [StringLength(250)]
        public string DataSource { get; set; }

        [StringLength(50)]
        public string DatabaseConnectionType { get; set; }

        public ConnectionType GetConnectionType()
        {
            return DatabaseConnectionType.ToEnum<ConnectionType>(true);
        }
    }
}
