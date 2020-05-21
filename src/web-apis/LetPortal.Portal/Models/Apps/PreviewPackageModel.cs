using System.Collections.Generic;
using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Models.Apps
{
    public class PreviewPackageModel
    {
        public App App { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Charts { get; set; }

        public IEnumerable<string> DynamicLists { get; set; }

        public IEnumerable<string> Pages { get; set; }

        public IEnumerable<string> Locales { get; set; }
    }
}
