using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.Pages;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.SectionParts
{
    [EntityCollection(Name = "components")]
    public class Component : BackupableEntity
    {     
        public string DatasourceName { get; set; }

        public List<ShellOption> Options { get; set; }

        public bool AllowOverrideOptions { get; set; }

        public bool AllowPassingDatasource { get; set; }
    }
}
