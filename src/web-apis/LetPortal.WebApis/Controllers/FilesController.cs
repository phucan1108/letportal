using LetPortal.Core.Logger;
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

        private readonly IServiceLogger<FilesController> _logger;

        public FilesController(
            IFileService fileService,
            IServiceLogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ResponseUploadFile), 200)]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            _logger.Info("Upload file with name = {name} size = {size}", formFile.FileName, formFile.Length);
            var result = await _fileService.UploadFileAsync(formFile, "", false);
            _logger.Info("Uploaded file: {@result}", result);
            return Ok(result);
        }

        [HttpGet("metadata/{fileId}")]
        [ProducesResponseType(typeof(File), 200)]
        public async Task<IActionResult> GetFileInfo(string fileId)
        {
            var result = await _fileService.GetFileInfo(fileId);
            _logger.Info("File info: {@result}", result);
            return Ok(result);
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> GetFile(string fileId, [FromQuery] bool? compress)
        {
            var response = await _fileService.DownloadFileAsync(fileId, compress.HasValue ? compress.Value : false);
            _logger.Info("Responsed file when downloading: {fileName} {size}", response.FileName, response.FileBytes.Length);
            return File(response.FileBytes, response.MIMEType, response.FileName);
        }
    }
}
