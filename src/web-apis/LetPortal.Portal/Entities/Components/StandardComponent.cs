using System.Collections.Generic;
using LetPortal.Portal.Entities.SectionParts.Controls;

namespace LetPortal.Portal.Entities.SectionParts
{
    public class StandardComponent : Component
    {
        public bool AllowArrayData { get; set; }

        public List<PageControl> Controls { get; set; }
    }
}
