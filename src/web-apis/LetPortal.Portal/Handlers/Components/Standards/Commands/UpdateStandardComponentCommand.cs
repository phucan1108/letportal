using LetPortal.Portal.Entities.SectionParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Components.Standards.Commands
{
    public class UpdateStandardComponentCommand
    {
        public string StandardId { get; set; }

        public StandardComponent Standard { get; set; }
    }
}
