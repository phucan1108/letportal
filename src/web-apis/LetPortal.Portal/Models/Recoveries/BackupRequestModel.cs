using System.Collections.Generic;

namespace LetPortal.Portal.Models.Recoveries
{
    public class BackupRequestModel
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

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string EncryptKey { get; set; }

    }
}
