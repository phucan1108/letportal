using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core
{
    class DatabaseOptionsBuilder : IDatabaseOptionsBuilder
    {
        public DatabaseOptionsBuilder(IServiceCollection services, IConfiguration configuration, DatabaseOptions databaseOptions)
        {
            Services = services;
            Configuration = configuration;
            DatabaseOptions = databaseOptions;
        }

        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }

        public DatabaseOptions DatabaseOptions { get; }
    }
}
