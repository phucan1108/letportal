using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Portal.Services.Apps;

namespace LetPortal.Portal.Models.Apps
{
    public class InstallRequestModel
    {
        public string UploadFileId { get; set; }

        public InstallWay InstallWay { get; set; }
    }
}
