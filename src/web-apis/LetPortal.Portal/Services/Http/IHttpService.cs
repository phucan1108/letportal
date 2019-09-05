using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Https;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Http
{
    public interface IHttpService
    {
        Task<HttpResultModel> ExecuteHttp(HttpServiceOptions httpServiceOptions);
    }
}
