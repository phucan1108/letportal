using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Databases
{
    public class ExecuteParamModel
    {
        public string Name { get; set; }

        public string ReplaceValue { get; set; }

        public bool RemoveQuotes { get; set; }
    }
}
