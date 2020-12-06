using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using LetPortal.Microservices.LogCollector;

namespace LetPortal.Microservices.Client.Channels
{
    class LogCollectorChannel : ILogCollectorChannel
    {
        private readonly Lazy<Channel<LogCollectorRequest>> _channelLazy 
            = new Lazy<Channel<LogCollectorRequest>>(() => Channel.CreateBounded<LogCollectorRequest>(1000));

        public Channel<LogCollectorRequest> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
