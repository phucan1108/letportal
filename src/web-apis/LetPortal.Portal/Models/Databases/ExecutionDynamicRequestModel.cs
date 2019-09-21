using System.Collections.Generic;

namespace LetPortal.Portal.Models.Databases
{
    public class ExecutionDynamicRequestModel
    {
        public string DatabaseId { get; set; }

        public string SourcePath { get; set; }

        public List<ExecuteParamModel> Params { get; set; }
    }
}
