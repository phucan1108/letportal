using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.Portal.Entities.EntitySchemas
{
    [EntityCollection(Name = EntitySchemaConstants.EntitySchemaCollection)]
    [Table("entityschemas")]
    public class EntitySchema : BackupableEntity
    {                                       
        public string DatabaseId { get; set; }

        public string AppId { get; set; }

        public List<EntityField> EntityFields { get; set; } = new List<EntityField>();
    }

    public class EntityField
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string FieldType { get; set; }
    }
}
