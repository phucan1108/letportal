using System.Threading.Tasks;
using LetPortal.Core.Persistences;

namespace LetPortal.Core.Versions
{
    public interface IVersionRepository : IGenericRepository<Version>
    {
        Task<Version> GetLastestVersion(string appName);
    }
}
