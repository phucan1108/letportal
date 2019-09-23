using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
