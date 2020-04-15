using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;

namespace LetPortal.Portal.Entities.Files
{
    [EntityCollection(Name = FileConstants.FileCollection)]
    [Table("files")]
    public class File : Entity
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Uploader { get; set; }

        [StringLength(250)]
        public string DownloadableUrl { get; set; }

        [StringLength(1000)]
        public string IdentifierOptions { get; set; }

        [StringLength(250)]
        public string MIMEType { get; set; }

        public long FileSize { get; set; }

        public bool AllowCompress { get; set; }

        public FileStorageType FileStorageType { get; set; }

        public DateTime DateUploaded { get; set; }
    }
}
