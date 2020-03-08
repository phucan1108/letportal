using System.IO;
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
    }
}
