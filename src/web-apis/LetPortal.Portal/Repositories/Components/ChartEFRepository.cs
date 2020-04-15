using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
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

        public async Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var charts = await _context.Charts.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();
                return charts?.AsEnumerable();
            }
            else
            {
                return (await _context.Charts.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync())?.AsEnumerable();
            }
        }
    }
}
