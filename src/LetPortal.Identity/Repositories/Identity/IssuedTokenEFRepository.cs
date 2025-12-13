using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public class IssuedTokenEFRepository : EFGenericRepository<IssuedToken>, IIssuedTokenRepository
    {
        private readonly IdentityDbContext _context;

        public IssuedTokenEFRepository(IdentityDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<bool> DeactiveRefreshToken(string refreshToken)
        {
            var stillAvailable = _context.IssuedTokens.Any(a => !a.Deactive && a.ExpiredRefreshToken > DateTime.UtcNow && a.RefreshToken == refreshToken);
            if (stillAvailable)
            {
                var token = _context.IssuedTokens.First(a => a.RefreshToken == refreshToken);
                token.Deactive = true;

                _context.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<IssuedToken> GetByRefreshToken(string refreshToken)
        {
            return Task.FromResult(_context.IssuedTokens.First(a => a.RefreshToken == refreshToken));
        }

        public Task<string> GetTokenByRefreshToken(string refreshToken)
        {
            return Task.FromResult(_context.IssuedTokens.First(a => a.RefreshToken == refreshToken && !a.Deactive).JwtToken);
        }
    }
}
