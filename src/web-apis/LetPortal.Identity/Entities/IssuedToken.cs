using System;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "issuedtokens")]
    [Table("issuedtokens")]
    public class IssuedToken : Entity
    {
        public string JwtToken { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public string RefreshToken { get; set; }

        public string UserSessionId { get; set; }

        public DateTime ExpiredJwtToken { get; set; }

        public DateTime ExpiredRefreshToken { get; set; }

        public bool Deactive { get; set; }
    }
}
