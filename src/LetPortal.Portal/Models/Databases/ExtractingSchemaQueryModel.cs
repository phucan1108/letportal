using System.Collections.Generic;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Models.Databases
{
    public class ExtractingSchemaQueryModel
    {
        public List<ColumnField> ColumnFields { get; set; } = new List<ColumnField>();
    }

}
