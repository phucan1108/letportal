using System.Threading.Tasks;
using LetPortal.Portal.Models.Apps;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Services.Apps
{
    public interface IAppService
    {
        Task<PackageResponseModel> Package(PackageRequestModel package);

        Task<UnpackResponseModel> UnPack(IFormFile uploadFile, string uploader);

        Task Install(string uploadFileId, bool overRide = false);
    }
}
