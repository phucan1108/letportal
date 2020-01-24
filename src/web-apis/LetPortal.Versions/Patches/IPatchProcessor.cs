using LetPortal.Core.Persistences;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Versions.Patches
{
    public interface IPatchProcessor
    {
        Task<IEnumerable<string>> Proceed(string folderPath, DatabaseOptions databaseOptions);
    }
}
