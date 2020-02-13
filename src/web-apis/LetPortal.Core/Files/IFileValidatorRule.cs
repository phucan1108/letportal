using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Files
{
    public interface IFileValidatorRule
    {
        Task Validate(IFormFile file, string tempFilePath);

        Task Validate(string filePath);
    }
}
