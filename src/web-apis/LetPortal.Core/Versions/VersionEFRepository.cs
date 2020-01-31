using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Core.Versions
{
    public class VersionEFRepository : EFGenericRepository<Version>, IVersionRepository
    {
        public VersionEFRepository(DbContext context) : base(context)
        {
        }
    }
}
