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
    class Downgrade : IFeatureCommand
    {
        public string CommandName => "down";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            if (toolsContext.LatestVersion != null)
            {
                var requestingVersionNumber = toolsContext.VersionNumber.ToVersionNumber();
                if (requestingVersionNumber < toolsContext.LatestVersion.GetNumber())
                {
                    var matchingVersions = toolsContext.Versions.Where(
                         a => a.GetNumber() <= toolsContext.LatestVersion.GetNumber()
                             && a.GetNumber() > requestingVersionNumber);

                    Console.WriteLine("----------------------DOWNGRADE PROGRESS------------------------");
                    Console.WriteLine("DOWNGRADE VERSION: " + matchingVersions.Last().VersionNumber);
                    Console.WriteLine("-----------------------++++++++++++++++-------------------------");

                    var portalVersions = await DowngradingVersion(matchingVersions, toolsContext);
                    foreach (var portalVersion in portalVersions)
                    {
                        var storedVersion = toolsContext.VersionRepository.GetAsQueryable().Where(a => a.VersionNumber == portalVersion.VersionNumber).FirstOrDefault();

                        if (storedVersion != null)
                        {
                            await toolsContext.VersionRepository.DeleteAsync(storedVersion.Id);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Oops we can't downgrade version because your request is {0} but the current is {1}", toolsContext.VersionNumber, toolsContext.LatestVersion.VersionNumber));
                }
            }
            else
            {
                Console.WriteLine("Oops we can't find any installation in the database, please use 'install' command before downgrading.");
            }
        }

        private async Task<List<Version>> DowngradingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<Version>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.OrderByDescending(b => b.GetNumber()).Select(a => a.VersionNumber).Distinct();

            foreach (var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Downgrading Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach (var version in matchingVersions)
                {
                    await version.Downgrade(toolsContext.VersionContext);
                    executingVersions.Add(version.GetType().GetTypeInfo().Name);
                    Console.WriteLine(string.Format("Downgrading {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
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
