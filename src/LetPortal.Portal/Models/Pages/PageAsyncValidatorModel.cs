using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Pages
{
    public class PageAsyncValidatorModel
    {            
        public string SectionName { get; set; }

        public string ControlName { get; set; }

        public string AsyncName { get; set; }

        public List<PageParameterModel> Parameters { get; set; }
    }
}
