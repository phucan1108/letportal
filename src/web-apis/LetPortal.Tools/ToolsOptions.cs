using LetPortal.Core.Persistences;

namespace LetPortal.Tools
{
    public class ToolsOptions
    {
        public StoringConnections StoringConnections { get; set; }
        public string PatchesFolderPath { get; set; }
    }

    public class StoringConnections
    {
        public DatabaseOptions PortalConnection { get; set; }
        public DatabaseOptions IdentityConnection { get; set; }
        public DatabaseOptions ServiceManagementConnection { get; set; }
    }
}
