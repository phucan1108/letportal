using System.Collections.Generic;
using LetPortal.Core.Persistences;

namespace LetPortal.Portal.Models.Recoveries
{
    public class PreviewRestoreModel
    {
        public IEnumerable<ComparisonResult> Apps { get; set; }

        public IEnumerable<ComparisonResult> Standards { get; set; }

        public IEnumerable<ComparisonResult> Tree { get; set; }

        public IEnumerable<ComparisonResult> Array { get; set; }

        public IEnumerable<ComparisonResult> Databases { get; set; }

        public IEnumerable<ComparisonResult> Charts { get; set; }

        public IEnumerable<ComparisonResult> DynamicLists { get; set; }

        public IEnumerable<ComparisonResult> Pages { get; set; }

        public IEnumerable<ComparisonResult> CompositeControls { get; set; }

        public int TotalObjects { get; set; }

        public int TotalChangedObjects { get; set; }

        public int TotalUnchangedObjects { get; set; }

        public int TotalNewObjects { get; set; }
    }
}
