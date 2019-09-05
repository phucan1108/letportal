using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetPortal.Portal.Handlers
{
    public static class HandlersRegistry
    {
        /// <summary>
        /// Register all handlers for MediatR
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddPortalHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
