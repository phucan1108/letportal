using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;

namespace LetPortal.Microservices.Server.Repositories.Abstractions
{
    public interface INotificationMessageQueueRepository : IGenericRepository<NotificationMessageQueue>
    {
    }
}
