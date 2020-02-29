using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Pages
{
    public class PageTriggeringEventModel
    {
        public string SectionName { get; set; }

        public string ControlName { get; set; }

        public string EventName { get; set; }

        public List<PageParameterModel> Parameters { get; set; }
    }
}
