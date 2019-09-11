using LetPortal.Core.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LetPortal.Tools.Features
{
    class Uninstall : IFeatureCommand
    {
        public string CommandName => "uninstall";

        public async Task RunAsync(ToolsContext context)
        {
            if(context.LatestVersion != null)
            {
                var requestingVersionNumber = context.LatestVersion.GetNumber();
                var matchingVersions = context.Versions.Where(a => a.GetNumber() <= requestingVersionNumber);
                UninstallingVersion(matchingVersions, context);
                var foundVersions = await context.PortalVersionRepository.GetAllAsync(isRequiredDiscriminator: false);
                await context.PortalVersionRepository.DeleteBulkAsync(foundVersions.Select(a => a.Id));
            }
            else
            {
                Console.WriteLine("Oops we don't find any installation in the database.");
            }
        }
        private void UninstallingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            foreach(var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Uninstalling Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach(var version in matchingVersions)
                {
                    version.Downgrade(toolsContext.VersionContext);
                    Console.WriteLine(string.Format("Uninstalling {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
                }
            }
        }
    }
}
