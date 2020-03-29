using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace LetPortal.Core.Security
{
    public static class SecurityExtensions
    {
        public static string GetUserName(this JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.Subject)?.Value;
        }

        public static string GetUserId(this JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.Id)?.Value;
        }

        public static string GetFullName(this JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.GivenName)?.Value;
        }

        public static string GetAvatar(this JwtSecurityToken token)
        {
            return token.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.Picture)?.Value;
        }
    }
}
