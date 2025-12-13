using System.Threading.Tasks;
using LetPortal.Portal.Exceptions.Recoveries;
using LetPortal.Portal.Models.Recoveries;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Services.Recoveries
{
    public interface IBackupService
    {
        /// <summary>
        /// Unzip a package and then restore a backup point
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        Task<UploadBackupResponseModel> UploadBackupFile(IFormFile uploadFile, string uploader);

        /// <summary>
        /// Create a json file and store it into FileService
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BackupResponseModel> CreateBackupFile(BackupRequestModel model);

        Task<PreviewRestoreModel> PreviewBackup(string backupId);

        Task RestoreBackupPoint(string backupId);

        Task<GenerateCodeResponseModel> CreateCode(GenerateCodeRequestModel model);
    }
}
