using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Https;

namespace LetPortal.Portal.Services.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResultModel> ExecuteHttp(HttpServiceOptions httpServiceOptions)
        {
            var successCodes = httpServiceOptions.HttpSuccessCode.Split(";").Select(a => int.Parse(a));
            var httpResult = new HttpResultModel();
            HttpResponseMessage response = null;
            switch (httpServiceOptions.HttpMethod)
            {
                case "Get":
                    response = await _httpClient.GetAsync(httpServiceOptions.HttpServiceUrl);
                    break;
                case "Post":
                    using (var stringContent = new StringContent(httpServiceOptions.JsonBody, Encoding.UTF8))
                    {
                        response = await _httpClient.PostAsync(httpServiceOptions.HttpServiceUrl, stringContent);
                    }
                    break;
                case "Put":
                    using (var stringContent = new StringContent(httpServiceOptions.JsonBody, Encoding.UTF8))
                    {
                        response = await _httpClient.PutAsync(httpServiceOptions.HttpServiceUrl, stringContent);
                    }
                    break;
                case "Delete":
                    response = await _httpClient.DeleteAsync(httpServiceOptions.HttpServiceUrl);
                    break;
            }

            if (successCodes.Any(a => a == (int)response.StatusCode))
            {
                httpResult.IsSuccess = true;
                httpResult.Result = await response.Content.ReadAsJsonAsync<dynamic>();
            }
            return httpResult;
        }
    }
}
