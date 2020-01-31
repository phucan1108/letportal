using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.Pages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.Portal.Entities.SectionParts
{
    [EntityCollection(Name = "components")]
    [Table("components")]
    public class Component : BackupableEntity
    {
        public string DatasourceName { get; set; }

        public List<ShellOption> Options { get; set; }

        public PageSectionLayoutType LayoutType { get; set; } = PageSectionLayoutType.OneColumn;

        public bool AllowOverrideOptions { get; set; }

        public bool AllowPassingDatasource { get; set; }
    }

    public enum PageSectionLayoutType
    {
        OneColumn,
        TwoColumns,
        ThreeColumns,
        FourColumns,
        SixColumns
    }
}
