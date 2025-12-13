using System.Collections.Generic;

namespace LetPortal.Portal.Models.Pages
{
    public class PageSubmittedButtonModel
    {
        public string ButtonName { get; set; }

        public List<PageParameterModel> Parameters { get; set; }

        public List<LoopDataModel> LoopDatas { get; set; }
    }

    public class LoopDataModel
    {
        public string Name { get; set; }
        
        public List<List<PageParameterModel>> Parameters { get; set; }
    }
}
