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

        public string Creator { get; set; }

        public DateTime PackagedDate { get; set; }

        public App App { get; set; }    

        public IEnumerable<ComponentInstallState> Standards { get; set; }
        
        public IEnumerable<ComponentInstallState> DynamicLists { get; set; }

        public IEnumerable<ComponentInstallState> Charts { get; set; }

        public IEnumerable<ComponentInstallState> Pages { get; set; }

        public IEnumerable<ComponentInstallState> Locales { get; set; }

        public int TotalStandards { get; set; }

        public int TotalCharts { get; set; }

        public int TotalDynamicLists { get; set; }

        public int TotalPages { get; set; }

        public int TotalLocales { get; set; }

        public bool IsExistedId { get; set; }

        public bool IsExistedName { get; set; }
    }

    public class ComponentInstallState
    {
        public string Name { get; set; }

        public bool IsExisted { get; set; }
    }
}
