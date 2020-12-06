using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Components
{
    public class CompositeControlEFRepository : EFGenericRepository<CompositeControl>, ICompositeControlRepository
    {
        private readonly PortalDbContext _context;

        public CompositeControlEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortEntities(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var standards = await _context.CompositeControls.Where(a => a.DisplayName.Contains(keyword)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName, AppId = b.AppId }).ToListAsync();
                return standards?.AsEnumerable();
            }
            else
            {
                return (await _context.CompositeControls.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).ToListAsync())?.AsEnumerable();
            }
        }
    }
}
