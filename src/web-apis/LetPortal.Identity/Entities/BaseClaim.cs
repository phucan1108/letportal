using System.Security.Claims;

namespace LetPortal.Identity.Entities
{
    public class BaseClaim
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string Issuer { get; set; } = StandardClaims.DefaultIssuer;

        public string ClaimValueType { get; set; } = StandardClaims.DefaultStringValueType;

        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue, ClaimValueType, Issuer);
        }
    }
}
