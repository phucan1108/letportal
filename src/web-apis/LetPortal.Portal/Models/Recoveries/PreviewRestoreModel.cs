using LetPortal.Core.Persistences;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Recoveries
{
    public class PreviewRestoreModel
    {
        public IEnumerable<ComparisonResult> Apps { get; set; }

        public IEnumerable<ComparisonResult> Standards { get; set; }

        public IEnumerable<ComparisonResult> Databases { get; set; }

        public IEnumerable<ComparisonResult> Charts { get; set; }

        public IEnumerable<ComparisonResult> DynamicLists { get; set; }

        public IEnumerable<ComparisonResult> Pages { get; set; }
    }
}
