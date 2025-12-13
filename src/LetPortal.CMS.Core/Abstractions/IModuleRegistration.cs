using System.Collections.Generic;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Core.Abstractions
{
    public interface IModuleRegistration
    {
        public string Name { get; }

        public string Description { get; }

        public IEnumerable<string> RequiredParts { get; }

        public void Register(
            IServiceCollection services, 
            IConfiguration configuration,
            DatabaseOptions databaseOptions);
    }
}
