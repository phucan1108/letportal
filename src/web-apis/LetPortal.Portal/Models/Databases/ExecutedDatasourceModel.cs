using System.Collections.Generic;

namespace LetPortal.Portal.Models
{
    public class ExecutedDataSourceModel
    {
        public List<DatasourceModel> DatasourceModels { get; set; }

        public bool CanCache { get; set; }
    }
}
