using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "issuedtokens")]
    public class IssuedToken : Entity
    {
        public string JwtToken { get; set; }

        public string UserId { get; set; }

        public string RefreshToken { get; set; }

        public string UserSessionId { get; set; }

        public DateTime ExpiredJwtToken { get; set; }

        public DateTime ExpiredRefreshToken { get; set; }

        public bool Deactive { get; set; }
    }
}
