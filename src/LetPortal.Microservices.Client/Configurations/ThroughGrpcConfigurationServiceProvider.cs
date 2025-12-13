using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LetPortal.Microservices.Configurations;
using Newtonsoft.Json.Linq;
using Polly;

namespace LetPortal.Microservices.Client.Configurations
{
    public class ThroughGrpcConfigurationServiceProvider : IConfigurationServiceProvider
    {
        private readonly ServiceOptions _serviceOptions;

        public ThroughGrpcConfigurationServiceProvider(ServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions;
        }

        public async Task<Dictionary<string, string>> GetConfiguration(string serviceName, string version)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = _serviceOptions.Name;
            }

            if (!string.IsNullOrEmpty(version))
            {
                version = _serviceOptions.Version;
            }
            var response = await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(_serviceOptions.RetryCount, retryAttempt => TimeSpan.FromMilliseconds(_serviceOptions.DelayRetry))
                .ExecuteAsync(async () =>
                {
                    if (_serviceOptions.BypassSSL)
                    {
                        using var httpHandler = new HttpClientHandler();
                        httpHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                        using var httpClient = new HttpClient(httpHandler);
                        using var channel = GrpcChannel.ForAddress(_serviceOptions.SaturnEndpoint,
                            new GrpcChannelOptions
                            {
                                HttpClient = httpClient
                            });
                        var client = new ServiceConfiguration.ServiceConfigurationClient(channel);
                        return await client.PullAsync(new ServiceConfigurationRequest { ServiceName = serviceName, Version = version });
                    }
                    else
                    {
                        using var channel = GrpcChannel.ForAddress(_serviceOptions.SaturnEndpoint,
                                new GrpcChannelOptions { });
                        var client = new ServiceConfiguration.ServiceConfigurationClient(channel);
                        return await client.PullAsync(new ServiceConfigurationRequest { ServiceName = serviceName, Version = version });
                    }
                });

            var jToken = JToken.Parse(response.ConfigurationContent);
            var dic = new Dictionary<string, string>();
            RecursiveFlatten(dic, jToken, "");
            return dic;
        }

        private void RecursiveFlatten(Dictionary<string, string> dic, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (var prop in token.Children<JProperty>())
                    {
                        RecursiveFlatten(dic, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    var index = 0;
                    foreach (var value in token.Children())
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
