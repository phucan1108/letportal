using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using System.Threading.Tasks;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindByNormalizedUsername(string normilizedName);
    }
}
