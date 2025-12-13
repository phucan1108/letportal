using System;
using System.Threading.Channels;
using LetPortal.Microservices.Monitors;

namespace LetPortal.Microservices.Client.Channels
{
    public class MonitorHeartBeatChannel : IMonitoHeartBeatChannel
    {
        private readonly Lazy<Channel<HealthCheckRequest>> _channelLazy = new Lazy<Channel<HealthCheckRequest>>(() => Channel.CreateBounded<HealthCheckRequest>(1000));

        public Channel<HealthCheckRequest> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
