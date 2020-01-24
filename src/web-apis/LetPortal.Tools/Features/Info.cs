using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Tools.Features
{
    class Info : IFeatureCommand
    {
        public string CommandName => "info";

        public Task RunAsync(ToolsContext context)
        {
            if(context.LatestVersion != null)
            {
                Console.WriteLine($"Current Version: {context.LatestVersion.VersionNumber}");                
                Console.WriteLine($"Last Modified Date: {context.LatestVersion.CreatedDate}");
            }
            else
            {
                Console.WriteLine("Oops we don't find any installation in the database.");
            }

            return Task.CompletedTask;
        }
    }
}
