using LetPortal.Core.Persistences;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core
{
    public static class ServiceExtensions
    {
        public static void AddMongoConfig(this IServiceCollection serviceCollection)
        {
            ConventionPackDefault.Register();
        }
    }
}
