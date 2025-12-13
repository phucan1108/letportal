using System.Collections.Generic;

namespace LetPortal.Portal.Models.Pages
{
    public class PageRequestDatasourceModel
    {
        public string DatasourceId { get; set; }


        public List<PageParameterModel> Parameters { get; set; }
    }
}
