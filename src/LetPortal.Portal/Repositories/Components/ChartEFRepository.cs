using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartEFRepository : EFGenericRepository<Chart>, IChartRepository
    {
        private readonly PortalDbContext _context;

        public ChartEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneChart = await _context.Charts.AsNoTracking().FirstAsync(a => a.Id == cloneId);
            cloneChart.Id = DataUtil.GenerateUniqueId();
            cloneChart.Name = cloneName;
            cloneChart.DisplayName += " Clone";
            await AddAsync(cloneChart);
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allCharts = await GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);

            var languages = new List<LanguageKey>();

            foreach(var chart in allCharts)
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

        public async Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var charts = await _context.Charts.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName, AppId = b.AppId }).ToListAsync();
                return charts?.AsEnumerable();
            }
            else
            {
                return (await _context.Charts.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).ToListAsync())?.AsEnumerable();
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
