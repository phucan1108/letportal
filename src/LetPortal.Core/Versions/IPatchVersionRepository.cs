using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;

namespace LetPortal.Core.Versions
{
    public interface IPatchVersionRepository : IGenericRepository<PatchVersion>
    {
        Task<PatchVersion> GetLatestAsync(string appName, string patchName);
    }
}
