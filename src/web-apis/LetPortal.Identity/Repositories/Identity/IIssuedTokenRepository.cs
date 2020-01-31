using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using System.Threading.Tasks;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IIssuedTokenRepository : IGenericRepository<IssuedToken>
    {
        Task<IssuedToken> GetByRefreshToken(string refreshToken);

        Task<string> GetTokenByRefreshToken(string refreshToken);

        Task<bool> DeactiveRefreshToken(string refreshToken);
    }
}
