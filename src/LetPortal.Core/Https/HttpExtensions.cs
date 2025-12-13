using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Https
{
    public static class HttpExtensions
    {
        public static string GetClientIpAddress(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static async Task<string> GetRawBodyAsync(this HttpRequest httpRequest)
        {
            try
            {
                using (var reader = new StreamReader(httpRequest.Body, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                return null;
            }

        }
        public static async Task<string> GetRawBodyAsync(this HttpResponse httpResponse)
        {
            try
            {
                using (var reader = new StreamReader(httpResponse.Body, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                return null;
            }
        }

        public static JwtSecurityToken GetJwtToken(this HttpRequest httpRequest)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtTokenStr = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty, System.StringComparison.Ordinal);
            var jwtToken = handler.ReadJwtToken(jwtTokenStr);
            return jwtToken;
        }
    }
}
