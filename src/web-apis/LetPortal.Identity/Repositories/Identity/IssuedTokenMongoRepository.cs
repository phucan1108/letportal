using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Identity.Repositories.Identity
{
    public class IssuedTokenMongoRepository : MongoGenericRepository<IssuedToken>, IIssuedTokenRepository
    {
        public IssuedTokenMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<bool> DeactiveRefreshToken(string refreshToken)
        {
            var stillAvailable = await Collection.AsQueryable().AnyAsync(a => !a.Deactive && a.ExpiredRefreshToken > DateTime.UtcNow && a.RefreshToken == refreshToken);
            if (stillAvailable)
            {
                var updateBuider = Builders<IssuedToken>.Update;
                var updateDefinition = updateBuider.Set(a => a.Deactive, true);
                await Collection.FindOneAndUpdateAsync(a => a.RefreshToken == refreshToken, updateDefinition);

                return true;
            }

            return false;
        }

        public async Task<IssuedToken> GetByRefreshToken(string refreshToken)
        {
            return await Collection.AsQueryable().FirstAsync(a => a.RefreshToken == refreshToken);
        }

        public async Task<string> GetTokenByRefreshToken(string refreshToken)
        {
            return (await Collection.AsQueryable().FirstAsync(a => a.RefreshToken == refreshToken)).JwtToken;
        }
    }
}
