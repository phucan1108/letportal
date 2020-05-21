using System;
using System.Threading.Tasks;
using LetPortal.Portal.Services.Apps;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Tools.Features
{
    class Unpack : IFeatureCommand
    {
        public string CommandName => "unpack";

        public async Task RunAsync(ToolsContext context)
        {
            Console.WriteLine("---------------------UNPACKAGE APP PROGRESS-----------------------");
            Console.WriteLine($"Package file: {context.Arguments.FilePath}, Install mode: {context.Arguments.UnpackMode}");
            var appService = context.Services.GetService<IAppService>();
            await appService.Unpack(context.Arguments.FilePath, context.Arguments.UnpackMode.ToLower() == "wipe" ? InstallWay.Wipe : InstallWay.Merge);
            Console.WriteLine($"Unpack successfully!!!");
            Console.WriteLine("-----------------------++++++++++++++++-------------------------");
        }
    }
}
