using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LetPortal.Core.Files
{
    public interface IFileValidatorRule
    {
        Task Validate(IFormFile file, string tempFilePath);

        Task Validate(string filePath);
    }
}
