using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartMongoRepository : MongoGenericRepository<Chart>, IChartRepository
    {
        public ChartMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneChart = await GetOneAsync(cloneId);
            cloneChart.Id = DataUtil.GenerateUniqueId();
            cloneChart.Name = cloneName;
            cloneChart.DisplayName += " Clone";
            await AddAsync(cloneChart);
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allCharts = await GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);

            var languages = new List<LanguageKey>();

            foreach (var chart in allCharts)
            {
                languages.AddRange(GetChartLanguages(chart));
            }

            return languages;
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string chartId)
        {
            var chart = await GetOneAsync(chartId);

            return GetChartLanguages(chart);
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<Chart>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<Chart>.Filter.Eq("_t", typeof(Chart).Name);
                var combineFilter = Builders<Chart>.Filter.And(discriminatorFilter, regexFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {                   
                var discriminatorFilter = Builders<Chart>.Filter.Eq("_t", typeof(Chart).Name);
                return Task.FromResult(Collection.Find(discriminatorFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }            
        }

        private List<LanguageKey> GetChartLanguages(Chart chart)
        {
            var languages = new List<LanguageKey>();

            var chartName = new LanguageKey
            {
                Key = $"charts.{chart.Name}.options.displayName",
                Value = chart.DisplayName
            };

            languages.Add(chartName);

            if (chart.ChartFilters != null && chart.ChartFilters.Count > 0)
            {
                foreach (var chartFilter in chart.ChartFilters)
                {
                    var chartFilterName = new LanguageKey
                    {
                        Key = $"charts.{chart.Name}.filters.{chartFilter.Name}.name",
                        Value = chartFilter.DisplayName
                    };
                    languages.Add(chartFilterName);
                }
            }

            return languages;
        }
    }
}
