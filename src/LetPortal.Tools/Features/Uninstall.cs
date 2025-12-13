using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.Core.Tools;
using LetPortal.Core.Versions;

namespace LetPortal.Tools.Features
{
    class Uninstall : IFeatureCommand
    {
        public string CommandName => "uninstall";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            if (toolsContext.LatestVersion != null)
            {
                var requestingVersionNumber = toolsContext.LatestVersion.GetNumber();
                var matchingVersions = toolsContext.Versions.Where(a => a.GetNumber() <= requestingVersionNumber);
                Console.WriteLine("----------------------UNINSTALL PROGRESS------------------------");
                Console.WriteLine("UNINSTALLING VERSION: " + matchingVersions.Last().VersionNumber);
                Console.WriteLine("-----------------------++++++++++++++++-------------------------");
                await UninstallingVersion(matchingVersions, toolsContext);
                var foundVersions = await toolsContext.VersionRepository.GetAllAsync(isRequiredDiscriminator: false);
                foreach (var version in foundVersions)
                {
                    await toolsContext.VersionRepository.DeleteAsync(version.Id);
                }
            }
            else
            {
                Console.WriteLine("Oops we don't find any installation in the database.");
            }
        }
        private async Task UninstallingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            foreach (var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Uninstalling Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach (var version in matchingVersions)
                {
                    await version.Downgrade(toolsContext.VersionContext);
                    Console.WriteLine(string.Format("Uninstalling {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
                }
            }
        }
    }
}
