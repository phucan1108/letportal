using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions
{
    public class ChartReportProjection : IChartReportProjection
    {
        public Task<object> ProjectionFromDataTable(DataTable dataTable, string mappringProjection)
        {
            var array = new JArray();
            // Check 'group' is in DT
            if (dataTable.Columns.Contains("group"))
            {
                var grouped = dataTable.AsEnumerable()
                                         .GroupBy(a => a.Field<string>("group"));
                foreach (var group in grouped)
                {
                    var data =
                        group
                            .Select(a => new { name = a.Field<object>("name"), value = a.Field<object>("value") })
                            .ToList();
                    var groupObject = JObject.FromObject(new
                    {
                        name = group.Key,
                        series = data
                    });

                    array.Add(groupObject);
                }
            }
            else
            {
                var jObjects = dataTable.AsEnumerable()
                            .Select(a => new { name = a.Field<object>("name"), value = a.Field<object>("value") })
                            .Select(b => JObject.FromObject(b));
                foreach (var jObject in jObjects)
                {
                    array.Add(jObject);
                }
            }

            var result = array.ToObject<object>();
            return Task.FromResult(result);
        }
    }
}
