using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Identity.Repositories.Identity
{
    public class UserEFRepository : EFGenericRepository<User>, IUserRepository
    {
        private readonly LetPortalIdentityDbContext _context;

        public UserEFRepository(LetPortalIdentityDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<User> FindByNormalizedUsername(string normilizedName)
        {
            return Task.FromResult(_context.Users.First(a => a.NormalizedUserName == normilizedName));
        }
    }
}
