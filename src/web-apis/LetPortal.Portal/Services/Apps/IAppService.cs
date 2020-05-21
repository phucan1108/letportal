using System.Threading.Tasks;
using LetPortal.Portal.Models.Apps;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Services.Apps
{
    public interface IAppService
    {
        Task<PackageResponseModel> Package(PackageRequestModel package);

        Task<UnpackResponseModel> Unpack(IFormFile uploadFile, string uploader);

        Task Install(string uploadFileId, InstallWay installWay = InstallWay.Merge);

        Task<PreviewPackageModel> Preview(string appId);

        #region Supports CLI
        Task<string> Package(string appName, string saveFolderPath);

        Task Unpack(string zipFilePath, InstallWay installWay = InstallWay.Merge);

        #endregion
    }

    public enum InstallWay
    {
        Merge,
        Wipe
    }
}
