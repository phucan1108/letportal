using System.Threading.Channels;
using LetPortal.Microservices.Client.Models;

namespace LetPortal.Microservices.Client.Channels
{
    public interface INotificationChannel
    {
        Channel<NotificationMessage> GetChannel();
    }
}
