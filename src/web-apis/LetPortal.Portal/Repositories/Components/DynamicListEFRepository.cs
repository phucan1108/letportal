using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Components
{
    public class DynamicListEFRepository : EFGenericRepository<DynamicList>, IDynamicListRepository
    {
        private readonly LetPortalDbContext _context;

        public DynamicListEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null)
        {
            if(!string.IsNullOrEmpty(keyWord))
            {
                var dynamicLists = await _context.DynamicLists.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();
                return dynamicLists?.AsEnumerable();
            }
            else
            {
                return (await _context.DynamicLists.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync())?.AsEnumerable();
            }
        }
    }
}
