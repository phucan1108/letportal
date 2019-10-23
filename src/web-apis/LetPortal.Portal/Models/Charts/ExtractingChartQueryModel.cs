using System.Collections.Generic;

namespace LetPortal.Portal.Models.Charts
{
    public class ExtractingChartQueryModel
    {
        public List<FilledParameterModel> Parameters { get; set; }

        public string Query { get; set; }

        public string DatabaseId { get; set; }
    }

    public class FilledParameterModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
