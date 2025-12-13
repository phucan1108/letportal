using System;
using System.Threading.Tasks;
using LetPortal.Core.Tools;

namespace LetPortal.Tools.Features
{
    class Info : IFeatureCommand
    {
        public string CommandName => "info";

        public Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            if (toolsContext.LatestVersion != null)
            {
                Console.WriteLine($"Current Version: {toolsContext.LatestVersion.VersionNumber}");
                Console.WriteLine($"Last Modified Date: {toolsContext.LatestVersion.CreatedDate}");
            }
            else
            {
                Console.WriteLine("Oops we don't find any installation in the database.");
            }

            return Task.CompletedTask;
        }
    }
}
