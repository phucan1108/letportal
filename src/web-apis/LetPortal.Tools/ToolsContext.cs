using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Versions;
using LetPortal.Portal.Repositories.PortalVersions;
using System.Collections.Generic;

namespace LetPortal.Tools
{
    public class ToolsContext
    {
        public IEnumerable<IVersion> Versions { get; set; }

        public IVersionContext VersionContext { get; set; }

        public PortalVersion LatestVersion { get; set; }

        public IPortalVersionRepository PortalVersionRepository { get; set; }

        public string VersionNumber { get; set; }
    }
}
