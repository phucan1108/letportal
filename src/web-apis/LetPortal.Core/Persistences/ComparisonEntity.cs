using System.Collections.Generic;

namespace LetPortal.Core.Persistences
{
    public class ComparisonEntity
    {
        public IEnumerable<ComparisonProperty> Properties { get; set; }
    }

    public class ComparisonResult
    {
        public ComparisonEntity Result { get; set; }

        public bool IsTotallyNew { get; set; }
    }
}
