using System;
using System.Net.Http;

namespace LetPortal.Core.Https
{
    public class HttpClientProvider
    {
        private static readonly Lazy<HttpClientProvider> _lazy = new Lazy<HttpClientProvider>(() => new HttpClientProvider());

        private readonly HttpClient httpClient = new HttpClient();

        private HttpClientProvider()
        {

        }

        public static HttpClientProvider Default => _lazy.Value;

        public HttpClient GetHttpClient()
        {
            return httpClient;
        }
    }
}
