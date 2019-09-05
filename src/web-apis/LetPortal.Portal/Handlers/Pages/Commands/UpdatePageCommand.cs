using LetPortal.Portal.Entities.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Pages.Commands
{
    public class UpdatePageCommand
    {
        public string PageId { get; set; }

        public Page Page { get; set; }
    }
}
