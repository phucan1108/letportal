using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Modules.Commands
{
    public class CreateModuleSnapshotCommand
    {
        public string ModuleId { get; set; }

        public string Version { get; set; }

        public List<string> DynamicFormIds { get; set; }

        public List<string> DynamicListIds { get; set; }

        public List<string> Datasources { get; set; }
    }
}
