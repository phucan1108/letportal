using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Portal.Entities.Recoveries
{
    [EntityCollection(Name = "backups")]
    [Table("backups")]
    public class Backup : Entity
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Creator { get; set; }

        [StringLength(250)]
        public string FileId { get; set; }

        [StringLength(500)]
        public string DownloadableUrl { get; set; }

        public BackupElements BackupElements { get; set; }

        public string EncryptShalt { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class BackupElements
    {
        public IEnumerable<string> Apps { get; set; }

        public IEnumerable<string> Databases { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Tree { get; set; }

        public IEnumerable<string> Array { get; set; }

        public IEnumerable<string> Charts { get; set; }

        public IEnumerable<string> DynamicLists { get; set; }

        public IEnumerable<string> Pages { get; set; }

        public IEnumerable<string> CompositeControls { get; set; }
    }
}
