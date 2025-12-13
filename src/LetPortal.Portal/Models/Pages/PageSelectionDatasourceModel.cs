using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Pages
{
    public class PageSelectionDatasourceModel
    {
        public string SectionName { get; set; }

        public string ControlName { get; set; }

        public string CompositeControlId { get; set; }

        public bool IsChildCompositeControl { get; set; }

        public List<PageParameterModel> Parameters { get; set; }
    }
}
