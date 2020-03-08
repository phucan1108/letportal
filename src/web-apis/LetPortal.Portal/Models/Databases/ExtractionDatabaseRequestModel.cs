using System.Collections.Generic;

namespace LetPortal.Portal.Models.Databases
{
    public class ExtractionDatabaseRequestModel
    {
        public dynamic Content { get; set; }

        public IEnumerable<ExecuteParamModel> Parameters { get; set; }
    }
}
