using System;
using System.Threading.Tasks;
using LetPortal.Core.Tools;
using LetPortal.Portal.Services.Apps;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Tools.Features
{
    class Pack : IFeatureCommand
    {
        public string CommandName => "pack";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            Console.WriteLine("---------------------PACKAGE APP PROGRESS-----------------------");
            Console.WriteLine($"App Name: {toolsContext.Arguments.Name}, Ouput: {toolsContext.Arguments.Output}");
            var appService = toolsContext.Services.GetService<IAppService>();
            var savePath = await appService.Package(toolsContext.Arguments.Name, toolsContext.Arguments.Output);
            Console.WriteLine($"Package successfully: {savePath}");
            Console.WriteLine("-----------------------++++++++++++++++-------------------------");
        }
    }
}
