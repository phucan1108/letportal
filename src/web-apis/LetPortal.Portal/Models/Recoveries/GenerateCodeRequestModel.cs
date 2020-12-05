using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Recoveries
{
    public class GenerateCodeRequestModel
    {
        public IEnumerable<string> Apps { get; set; }

        public IEnumerable<string> Databases { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Tree { get; set; }

        public IEnumerable<string> Array { get; set; }

        public IEnumerable<string> DynamicLists { get; set; }

        public IEnumerable<string> Charts { get; set; }

        public IEnumerable<string> Pages { get; set; }

        public IEnumerable<string> CompositeControls { get; set; }

        public string FileName { get; set; }

        public string VersionNumber { get; set; }
    }
}
