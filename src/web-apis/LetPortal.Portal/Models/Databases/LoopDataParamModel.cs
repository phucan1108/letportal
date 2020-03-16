using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Databases
{
    public class LoopDataParamModel
    {
        public string Name { get; set; }

        public List<List<ExecuteParamModel>> Parameters { get; set; }
    }
}
