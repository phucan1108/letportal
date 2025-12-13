using System.Threading.Channels;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Client.Channels
{
    public interface IMonitoHeartBeatChannel
    {
        Channel<HealthCheckRequest> GetChannel();
    }
}
