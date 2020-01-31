using LetPortal.Core.Https;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LetPortal.Core.Configurations
{
    public class ThroughHttpConfigurationServiceProvider : IConfigurationServiceProvider
    {
        private readonly ConfigurationServiceOptions _configurationServiceOptions;

        public ThroughHttpConfigurationServiceProvider(ConfigurationServiceOptions configurationServiceOptions)
        {
            _configurationServiceOptions = configurationServiceOptions;
        }

        public async Task<Dictionary<string, string>> GetConfiguration(string serviceName, string version)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var response = await Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(rep => !rep.IsSuccessStatusCode)
                .WaitAndRetryAsync(_configurationServiceOptions.RetryCount, retryAttempt => TimeSpan.FromMilliseconds(_configurationServiceOptions.DelayRetry))
                .ExecuteAsync(async () =>
               {
                   var httpClient = HttpClientProvider.Default.GetHttpClient();

                   return await httpClient.GetAsync($"{_configurationServiceOptions.Endpoint}/{serviceName}/{version}");
               });

            var content = await response.Content.ReadAsStringAsync();

            var jToken = JToken.Parse(content);
            dic = new Dictionary<string, string>();
            RecursiveFlatten(dic, jToken, "");

            return dic;
        }

        private void RecursiveFlatten(Dictionary<string, string> dic, JToken token, string prefix)
        {
            switch(token.Type)
            {
                case JTokenType.Object:
                    foreach(JProperty prop in token.Children<JProperty>())
                    {
                        RecursiveFlatten(dic, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach(JToken value in token.Children())
                    {
                        RecursiveFlatten(dic, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    dic.Add(prefix, ((JValue)token).Value.ToString());
                    break;
            }
        }

        private string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + ":" + name);
        }
    }
}
