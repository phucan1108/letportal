using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartEFRepository : EFGenericRepository<Chart>, IChartRepository
    {
        private readonly LetPortalDbContext _context;

        public ChartEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortCharts(string keyWord = null)
        {
            if(!string.IsNullOrEmpty(keyWord))
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
