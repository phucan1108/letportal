using LetPortal.Portal.Entities.Apps;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Apps
{
    public interface IAppService
    {
        Task<App> CreateApp(App app);
    }
}
