using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core
{
    public interface IDatabaseOptionsBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }

        DatabaseOptions DatabaseOptions { get; }
    }
}
