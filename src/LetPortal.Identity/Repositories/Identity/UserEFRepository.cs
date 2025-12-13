using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public class UserEFRepository : EFGenericRepository<User>, IUserRepository
    {
        private readonly IdentityDbContext _context;

        public UserEFRepository(IdentityDbContext context)
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
