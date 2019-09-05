using LetPortal.Portal.Entities.SectionParts.Controls;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.SectionParts
{
    public class StandardComponent : Component
    {
        public PageSectionLayoutType LayoutType { get; set; }

        public List<PageControl> Controls { get; set; }
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
