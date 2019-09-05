using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;

namespace LetPortal.Portal.Databases.Models
{
    public class ParsingRawQueryModel
    {
        public List<EntityField> EntityFields { get; set; } = new List<EntityField>();
    }
}
