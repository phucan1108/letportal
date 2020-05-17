using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Models.Apps
{
    public class UnpackResponseModel
    {
        public string UploadFileId { get; set; }

        public string Description { get; set; }

        public DateTime PackagedDate { get; set; }

        public App App { get; set; }

        public bool IsExistedId { get; set; }

        public bool IsExistedName { get; set; }
    }
}
