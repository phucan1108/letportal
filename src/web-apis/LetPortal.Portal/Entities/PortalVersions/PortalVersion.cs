using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.Portal.Entities.Versions
{
    [EntityCollection(Name = "portalversions")]
    [Table("portalversions")]
    public class PortalVersion : Entity
    {
        public string VersionNumber { get; set; }

        public string AffectiveList { get; set; }

        public DateTime CreatedDate { get; set; }  

        public int GetNumber()
        {
            var splitted = VersionNumber.Split(".");
            string majorNumber = splitted[0];
            string minorNumber = splitted[1].Length == 1 ? "00" + splitted[1] : splitted[1].Length == 2 ? "0" + splitted[1] : splitted[1];
            string patchNumber = splitted[2].Length == 1 ? "00" + splitted[2] : splitted[2].Length == 2 ? "0" + splitted[2] : splitted[2];
            string number = majorNumber + minorNumber + patchNumber;
            return int.Parse(number);
        }
    }
}

