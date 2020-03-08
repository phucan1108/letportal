using System.Collections.Generic;

namespace LetPortal.Core.Persistences
{
    public class ComparisonEntity
    {
        public IList<ComparisonProperty> Properties { get; set; }
    }

    public class ComparisonResult
    {
        public ComparisonEntity Result { get; set; }

        public bool IsUnchanged { get; set; }

        public bool IsTotallyNew { get; set; }
    }
}
