using LetPortal.Core.Security;
using System.Collections.Generic;

namespace LetPortal.Portal.Models.Pages
{
    public class ShortPortalClaimModel
    {
        public string PageName { get; set; }

        public string PageDisplayName { get; set; }

        public List<PortalClaim> Claims { get; set; }
    }
}
