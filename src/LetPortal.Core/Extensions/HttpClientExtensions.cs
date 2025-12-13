using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonConvert.SerializeObject(data);
#pragma warning disable CA2000 // Dispose objects before losing scope
        var content = new StringContent(dataAsString);
#pragma warning restore CA2000 // Dispose objects before losing scope
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return httpClient?.PostAsync(new Uri(url), content);
    }

    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonConvert.SerializeObject(data);
#pragma warning disable CA2000 // Dispose objects before losing scope
        var content = new StringContent(dataAsString);
#pragma warning restore CA2000 // Dispose objects before losing scope
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return httpClient?.PutAsync(new Uri(url), content);
    }

    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        _ = content ?? throw new System.ArgumentNullException(nameof(content));
        var dataAsString = await content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonConvert.DeserializeObject<T>(dataAsString);
    }
}
