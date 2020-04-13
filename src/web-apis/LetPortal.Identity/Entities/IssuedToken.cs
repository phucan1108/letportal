using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "issuedtokens")]
    [Table("issuedtokens")]
    public class IssuedToken : Entity
    {
        [StringLength(4000)]
        public string JwtToken { get; set; }

        [StringLength(50)]
        public string UserId { get; set; }

        public User User { get; set; }

        [StringLength(4000)]
        public string RefreshToken { get; set; }

        [StringLength(50)]
        public string UserSessionId { get; set; }

        public DateTime ExpiredJwtToken { get; set; }

        public DateTime ExpiredRefreshToken { get; set; }

        public bool Deactive { get; set; }
    }
}
