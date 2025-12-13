using System;
using System.Threading.Channels;
using LetPortal.Microservices.LogCollector;

namespace LetPortal.Microservices.Client.Channels
{
    public interface ILogCollectorChannel
    {
        Channel<LogCollectorRequest> GetChannel();
    }
}
