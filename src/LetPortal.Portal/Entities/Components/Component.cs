using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Portal.Entities.SectionParts
{
    [EntityCollection(Name = "components")]
    [Table("components")]
    public class Component : BackupableEntity
    {
        [StringLength(250)]
        public string DatasourceName { get; set; }

        [StringLength(50)]
        public string AppId { get; set; }

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
