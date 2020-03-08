using System.Collections.Generic;

namespace LetPortal.Portal.Models.Pages
{
    public class PageSubmittedButtonModel
    {
        public string ButtonName { get; set; }

        public List<PageParameterModel> Parameters { get; set; }
    }
}
