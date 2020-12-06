using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.Core.Tools;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using Version = LetPortal.Core.Versions.Version;

namespace LetPortal.Tools.Features
{
    class Upgrade : IFeatureCommand
    {
        public string CommandName => "up";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            if (toolsContext.LatestVersion != null)
            {
                var requestingVersionNumber = toolsContext.VersionNumber.ToVersionNumber();
                if (toolsContext.LatestVersion.GetNumber() < requestingVersionNumber)
                {
                    var matchingVersions = toolsContext.Versions.Where(
                        a => a.GetNumber() > toolsContext.LatestVersion.GetNumber()
                            && a.GetNumber() <= requestingVersionNumber);
                    Console.WriteLine("---------------------UPGRADE PROGRESS-----------------------");
                    Console.WriteLine("UPGRADE VERSION: " + matchingVersions.Last().VersionNumber);
                    Console.WriteLine("-----------------------++++++++++++++++-------------------------");

                    var portalVersions = await UpgradingVersion(matchingVersions, toolsContext);
                    foreach (var portalVersion in portalVersions)
                    {
                        await toolsContext.VersionRepository.AddAsync(portalVersion);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Oops we can't upgrade version because your request is {0} but the current is {1}", toolsContext.VersionNumber, toolsContext.LatestVersion.VersionNumber));
                }
            }
            else
            {
                Console.WriteLine("Oops we can't find any installation in the database, please use 'install' command before upgrading.");
            }
        }

        private async Task<List<Version>> UpgradingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<Version>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            foreach (var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Upgrading Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach (var version in matchingVersions)
                {
                    await version.Upgrade(toolsContext.VersionContext);
                    executingVersions.Add(version.GetType().GetTypeInfo().Name);
                    Console.WriteLine(string.Format("Upgrading {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
                }

                dicVersions.Add(groupVersion, executingVersions);
            }

            foreach (var kvp in dicVersions)
            {
                effectivePortalVersions.Add(new Version
                {
                    Id = DataUtil.GenerateUniqueId(),
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    CreatedDate = DateTime.UtcNow
                });
            }

            return effectivePortalVersions;
        }
    }
}
