using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Core.Versions
{
    [EntityCollection(Name = "versions")]
    [Table("versions")]
    public class Version : Entity
    {
        [StringLength(100)]
        [Required]
        public string VersionNumber { get; set; }

        [StringLength(100)]
        [Required]
        public string AppName { get; set; }

        public string AffectiveFiles { get; set; }

        public string PatchFiles { get; set; }

        public string Executor { get; set; } = "CLI";

        public DateTime CreatedDate { get; set; }

        public int GetNumber()
        {
            var splitted = VersionNumber.Split(".");
            var majorNumber = splitted[0];
            var minorNumber = splitted[1].Length == 1 ? "00" + splitted[1] : splitted[1].Length == 2 ? "0" + splitted[1] : splitted[1];
            var patchNumber = splitted[2].Length == 1 ? "00" + splitted[2] : splitted[2].Length == 2 ? "0" + splitted[2] : splitted[2];
            var number = majorNumber + minorNumber + patchNumber;
            return int.Parse(number);
        }
    }
}
