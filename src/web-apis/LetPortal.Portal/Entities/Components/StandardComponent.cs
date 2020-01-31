using System.Collections.Generic;
using LetPortal.Portal.Entities.SectionParts.Controls;

namespace LetPortal.Portal.Entities.SectionParts
{
    public class StandardComponent : Component
    {
        public List<PageControl> Controls { get; set; }
    }
}
