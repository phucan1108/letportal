using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Microservices.Client.Models;

namespace LetPortal.Microservices.Client.Services
{
    public interface INotificationClientService
    {
        Task CreateChannel(string name, string code, string icon);

        Task Send(NotificationMessage notificationMessage);
    }
}
