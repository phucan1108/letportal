using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindByNormalizedUsername(string normilizedName);
    }
}
