using LetPortal.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IServiceManagementProvider
    {
        Task<string> RegisterService(RegisterServiceModel registerServiceModel);

        Task UpdateRunningState(string serviceId);

        Task ShutdownService(string serviceId);

        Task ShutdownAllServices();
    }
}
