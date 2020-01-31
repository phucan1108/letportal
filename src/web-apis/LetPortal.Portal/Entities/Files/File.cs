using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.Portal.Entities.Files
{
    [EntityCollection(Name = FileConstants.FileCollection)]
    [Table("files")]
    public class File : Entity
    {
        public string Name { get; set; }

        public string Uploader { get; set; }

        public string DownloadableUrl { get; set; }

        public string IdentifierOptions { get; set; }

        public string MIMEType { get; set; }

        public long FileSize { get; set; }

        public bool AllowCompress { get; set; }

        public FileStorageType FileStorageType { get; set; }

        public DateTime DateUploaded { get; set; }
    }
}
