using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LetPortal.Tools.Features
{
    class Upgrade : IFeatureCommand
    {
        public string CommandName => "up";

        public async Task RunAsync(ToolsContext context)
        {
            if(context.LatestVersion != null)
            {
                var requestingVersionNumber = context.VersionNumber.ToVersionNumber();
                if(context.LatestVersion.GetNumber() < requestingVersionNumber)
                {
                    var matchingVersions = context.Versions.Where(
                        a => a.GetNumber() > context.LatestVersion.GetNumber()
                            && a.GetNumber() <= requestingVersionNumber);

                    var portalVersions = UpgradingVersion(matchingVersions, context);
                    foreach(var portalVersion in portalVersions)
                    {
                        await context.PortalVersionRepository.AddAsync(portalVersion);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Oops we can't upgrade version because your request is {0} but the current is {1}", context.VersionNumber, context.LatestVersion.VersionNumber));
                }
            }
            else
            {
                Console.WriteLine("Oops we can't find any installation in the database, please use 'install' command before upgrading.");
            }
        }

        private List<PortalVersion> UpgradingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<PortalVersion>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            foreach(var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Installing Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach(var version in matchingVersions)
                {
                    version.Upgrade(toolsContext.VersionContext);
                    executingVersions.Add(version.GetType().GetTypeInfo().Name);
                    Console.WriteLine(string.Format("Installing {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
                }

                dicVersions.Add(groupVersion, executingVersions);
            }

            foreach(var kvp in dicVersions)
            {
                effectivePortalVersions.Add(new PortalVersion
                {
                    Id = DataUtil.GenerateUniqueId(),
                    VersionNumber = kvp.Key,
                    AffectiveList = ConvertUtil.SerializeObject(kvp.Value),
                    CreatedDate = DateTime.UtcNow,
                    RunningType = VersionRunningType.Upgrade
                });
            }

            return effectivePortalVersions;
        }
    }
}
