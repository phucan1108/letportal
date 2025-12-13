using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Models
{
    public class PortalClaimModel
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public BaseClaim ToBaseClaim()
        {
            return new BaseClaim
            {
                Issuer = StandardClaims.DefaultIssuer,
                ClaimType = Name,
                ClaimValue = Value,
                ClaimValueType = StandardClaims.DefaultStringValueType
            };
        }
    }
}
