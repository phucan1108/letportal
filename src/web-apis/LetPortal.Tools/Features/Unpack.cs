using System;
using System.Threading.Tasks;
using LetPortal.Core.Tools;
using LetPortal.Portal.Services.Apps;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Tools.Features
{
    class Unpack : IFeatureCommand
    {
        public string CommandName => "unpack";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            Console.WriteLine("---------------------UNPACKAGE APP PROGRESS-----------------------");
            Console.WriteLine($"Package file: {toolsContext.Arguments.FilePath}, Install mode: {toolsContext.Arguments.UnpackMode}");
            var appService = toolsContext.Services.GetService<IAppService>();
            await appService.Unpack(toolsContext.Arguments.FilePath, toolsContext.Arguments.UnpackMode.ToLower() == "wipe" ? InstallWay.Wipe : InstallWay.Merge);
            Console.WriteLine($"Unpack successfully!!!");
            Console.WriteLine("-----------------------++++++++++++++++-------------------------");
        }
    }
}
