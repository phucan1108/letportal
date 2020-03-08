using System.Threading.Tasks;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Https;

namespace LetPortal.Portal.Services.Http
{
    public interface IHttpService
    {
        Task<HttpResultModel> ExecuteHttp(HttpServiceOptions httpServiceOptions);
    }
}
