using LetPortal.Portal.Models.Shared;
using System.Collections.Generic;

namespace LetPortal.Portal.Models.Databases
{
    public class ExtractingSchemaQueryModel
    {
        public List<ColumnField> ColumnFields { get; set; } = new List<ColumnField>();
    }

}
