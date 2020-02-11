using System.Collections.Generic;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Databases.Models
{
    public class ParsingRawQueryModel
    {
        public List<EntityField> EntityFields { get; set; } = new List<EntityField>();
    }
}
