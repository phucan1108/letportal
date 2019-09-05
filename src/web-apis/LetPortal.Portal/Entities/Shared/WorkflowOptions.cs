using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Shared
{
    public class WorkflowOptions
    {
        public string WorkflowId { get; set; }

        public List<MapWorkflowInput> MapWorkflowInputs { get; set; }
    }
}
