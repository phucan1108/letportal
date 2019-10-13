using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                using(StreamReader reader = new StreamReader(httpRequest.Body, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
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
                using(StreamReader reader = new StreamReader(httpResponse.Body, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
