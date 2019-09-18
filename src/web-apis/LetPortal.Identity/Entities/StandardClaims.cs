using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Identity.Entities
{
    public class StandardClaims
    {
        public const string DefaultIssuer = "LetPortal";

        public const string DefaultStringValueType = "String";

        public const string DefaultArrayValueType = "Array";

        public static readonly BaseClaim AccessAppSelectorPage = new BaseClaim
        {
            Issuer = DefaultIssuer,
            ClaimType = "app_selector_page",
            ClaimValue = "access",
            ClaimValueType = DefaultStringValueType
        };

        public static BaseClaim AccessCoreApp(string coreAppId)
        {
            return new BaseClaim
            {
                Issuer = DefaultIssuer,
                ClaimType = "apps",
                ClaimValue = coreAppId,
                ClaimValueType = DefaultArrayValueType
            };
        }

        public static IEnumerable<BaseClaim> TransformRoleClaims(List<string> roles)
        {
            foreach (var role in roles)
            {
                yield return new BaseClaim
                {
                    Issuer = DefaultIssuer,
                    ClaimType = "roles",
                    ClaimValueType = DefaultArrayValueType,
                    ClaimValue = role
                };
            }
        }

        public static BaseClaim Sub(string userName)
        {
            return new BaseClaim
            {
                Issuer = DefaultIssuer,
                ClaimType = JwtClaimTypes.Subject,
                ClaimValue = userName,
                ClaimValueType = DefaultStringValueType
            };
        }

        public static BaseClaim UserId(string userId)
        {
            return new BaseClaim
            {
                Issuer = DefaultIssuer,
                ClaimType = JwtClaimTypes.Id,
                ClaimValue = userId,
                ClaimValueType = DefaultStringValueType
            };
        }

        public static BaseClaim Name(string username)
        {
            return new BaseClaim
            {
                Issuer = DefaultIssuer,
                ClaimType = JwtClaimTypes.Name,
                ClaimValue = username,
                ClaimValueType = DefaultStringValueType
            };
        }
    }
}
