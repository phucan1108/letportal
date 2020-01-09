using LetPortal.Core.Logger;
using LetPortal.Portal.Exceptions.Recoveries;
using LetPortal.Portal.Models.Recoveries;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LetPortal.PortalApis.Controllers
{
    [Route("api/backups")]
    [ApiController]
    public class BackupsController : ControllerBase
    {
        private readonly IBackupService _backupService;

        private readonly IBackupRepository _backupRepository;

        private readonly IServiceLogger<BackupsController> _logger;

        public BackupsController(
            IBackupService backupService,
            IBackupRepository backupRepository,
            IServiceLogger<BackupsController> logger)
        {
            _backupRepository = backupRepository;
            _backupService = backupService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(UploadBackupResponseModel), 200)]
        public async Task<IActionResult> UploadBackupFile(IFormFile formFile)
        {
            _logger.Info("Upload file with name = {name} size = {size}", formFile.FileName, formFile.Length);
            var result = await _backupService.UploadBackupFile(formFile, "");
            _logger.Info("Uploaded file: {@result}", result);
            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(BackupResponseModel), 200)]
        public async Task<IActionResult> Create([FromBody] BackupRequestModel model)
        {
            _logger.Info("Create backup {@model}", model);
            var result = await _backupService.CreateBackupFile(model);
            _logger.Info("Create backup completely {@result}", result);
            return Ok(result);
        }

        [HttpGet("{id}/preview")]
        [ProducesResponseType(typeof(PreviewRestoreModel), 200)]
        public async Task<IActionResult> PreviewBackup(string id)
        {
            _logger.Info("Preview backup id = {id}", id);
            var result = await _backupService.PreviewBackup(id);
            _logger.Info("Response preview backup: {@result}", result);
            return Ok(result);
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreBackup([FromBody] RestoreRequestModel model)
        {
            _logger.Info("Request restore with data: {@request}", model);
            await _backupService.RestoreBackupPoint(model.Id);
            _logger.Info("Restore complete for backup id = {@backupId}", model.Id);
            return Ok();
        }
    }
}
