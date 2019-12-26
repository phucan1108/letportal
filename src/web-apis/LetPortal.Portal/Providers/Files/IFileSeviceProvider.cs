using LetPortal.Portal.Models.Files;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Files
{
    public interface IFileSeviceProvider
    {
        Task<ResponseUploadFile> UploadFileAsync(string localFilePath, string uploader);
    }
}
