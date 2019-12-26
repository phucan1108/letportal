using LetPortal.Portal.Models.Recoveries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Recoveries
{
    public interface IBackupService
    {
        Task<BackupResponseModel> CreateBackupFile(BackupRequestModel model);   
    }
}
