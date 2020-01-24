using LetPortal.Core.Versions;
using System.Collections.Generic;

namespace LetPortal.Tools
{
    public class ToolsContext
    {
        public IEnumerable<IVersion> Versions { get; set; }

        public IVersionContext VersionContext { get; set; }

        public Version LatestVersion { get; set; }

        public IVersionRepository VersionRepository { get; set; }

        public string VersionNumber { get; set; }

        public string PatchesFolder { get; set; }

        public bool AllowPatch { get; set; }
    }
}
