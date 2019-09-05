using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Identity.Models
{
    public class RolePortalClaimModel
    {
        public string Name { get; set; }

        public List<string> Claims { get; set; }
    }
}
