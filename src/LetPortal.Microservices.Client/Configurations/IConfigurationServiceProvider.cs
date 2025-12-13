using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Microservices.Client.Configurations
{
    public interface IConfigurationServiceProvider
    {
        Task<Dictionary<string, string>> GetConfiguration(string serviceName, string version);
    }
}
