﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Apps
{
    public interface IAppRepository : IGenericRepository<App>
    {
        Task CloneAsync(string cloneId, string cloneName);

        Task UpdateMenuAsync(string appId, List<Menu> menus);

        Task UpdateMenuProfileAsync(string appId, MenuProfile menuProfile);

        Task<IEnumerable<ShortEntityModel>> GetShortApps(string keyWord = null);
    }
}
