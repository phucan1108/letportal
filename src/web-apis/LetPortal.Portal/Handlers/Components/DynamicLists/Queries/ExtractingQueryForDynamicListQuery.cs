using LetPortal.Portal.Models.DynamicLists;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Queries
{
    public class ExtractingQueryForDynamicListQuery
    {
        public List<FilledParameterModel> Parameters { get; set; }

        public string Query { get; set; }

        public string DatabaseId { get; set; }
    }
}
