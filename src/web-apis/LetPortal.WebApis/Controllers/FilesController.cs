using LetPortal.Portal.Entities.Files;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Services.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ResponseUploadFile), 200)]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            var result = await _fileService.UploadFileAsync(formFile, "");
            return Ok(result);
        }

        [HttpGet("metadata/{fileId}")]
        [ProducesResponseType(typeof(File), 200)]
        public async Task<IActionResult> GetFileInfo(string fileId)
        {
            return Ok(await _fileService.GetFileInfo(fileId));
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> GetFile(string fileId)
        {
            var response = await _fileService.DownloadFileAsync(fileId);
            return File(response.FileBytes, response.MIMEType, response.FileName);
        }
    }
}
