using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Menus.Repositories;
using LetPortal.CMS.Features.Menus.Resolvers;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Features.Menus
{
    public class MenuModuleRegistration : IModuleRegistration
    {
        public string Name => "MenuModule";

        public string Description => "Menu Module";

        public IEnumerable<string> RequiredParts => new List<string>
        {
            "MenuSection"
        };

        public void Register(IServiceCollection services, IConfiguration configuration, DatabaseOptions databaseOptions)
        {
            if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddSingleton<ISiteMenuRepository, SiteMenuMongoRepository>();
            }

            services.AddSingleton<ISiteRouteGlobalResolver, SiteMenuGlobalResolver>();
        }
    }
}
