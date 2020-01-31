using System;

namespace LetPortal.Portal.Exceptions.Recoveries
{
    public class UploadBackupResponseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public int TotalObjects { get; set; }

        public bool IsFileValid { get; set; }
    }
}
