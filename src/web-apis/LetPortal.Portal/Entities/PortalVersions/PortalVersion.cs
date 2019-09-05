using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System;

namespace LetPortal.Portal.Entities.Versions
{
    [EntityCollection(Name = "portalversions")]
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
            string patchNumber = splitted[0].Length == 1 ? "00" + splitted[0] : splitted[0].Length == 2 ? "0" + splitted[0] : splitted[0];
            string number = majorNumber + minorNumber + patchNumber;
            return int.Parse(number);
        }
    }
}

