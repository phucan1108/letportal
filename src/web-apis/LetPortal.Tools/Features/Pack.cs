using System;
using System.Threading.Tasks;
using LetPortal.Portal.Services.Apps;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Tools.Features
{
    class Pack : IFeatureCommand
    {
        public string CommandName => "pack";

        public async Task RunAsync(ToolsContext context)
        {
            Console.WriteLine("---------------------PACKAGE APP PROGRESS-----------------------");
            Console.WriteLine($"App Name: {context.Arguments.Name}, Ouput: {context.Arguments.Output}");
            var appService = context.Services.GetService<IAppService>();
            var savePath = await appService.Package(context.Arguments.Name, context.Arguments.Output);
            Console.WriteLine($"Package successfully: {savePath}");
            Console.WriteLine("-----------------------++++++++++++++++-------------------------");
        }
    }
}
