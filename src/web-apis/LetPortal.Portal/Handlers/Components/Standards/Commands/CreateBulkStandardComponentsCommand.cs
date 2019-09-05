using LetPortal.Portal.Entities.SectionParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Components.Standards.Commands
{
    public class CreateBulkStandardComponentsCommand
    {
        public List<StandardComponent> Standards { get; set; }
    }
}
