using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public class ChartReportProjection : IChartReportProjection
    {
        public Task<dynamic> ProjectionFromDataTable(DataTable dataTable, string mappringProjection)
        {
            var grouped = dataTable.AsEnumerable()
                                        .GroupBy(a => a.Field<string>("group"));
            JArray @array = new JArray();
            foreach(var group in grouped)
            {
                var data =
                    group
                        .Select(a => new { name = a.Field<string>("name"), value = a.Field<object>("value") })
                        .ToList();
                JObject groupObject = JObject.FromObject(new
                {
                    name = group.Key,
                    series = data
                });

                @array.Add(groupObject);
            }

            return @array.ToObject<dynamic>();
        }
    }
}
