using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;

namespace LetPortal.Versions.Patches
{
    public interface IPatchProcessor
    {
        Task<IEnumerable<string>> Proceed(string folderPath, DatabaseOptions databaseOptions);
    }
}
