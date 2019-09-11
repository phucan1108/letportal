using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.DynamicLists
{
    public class ExtractingQueryModel
    {
        public List<FilledParameterModel> Parameters { get; set; }

        public string Query { get; set; }

        public string DatabaseId { get; set; }
    }
}
