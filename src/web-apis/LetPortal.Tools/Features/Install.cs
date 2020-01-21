using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Version = LetPortal.Core.Versions.Version;

namespace LetPortal.Tools.Features
{
    class Install : IFeatureCommand
    {
        public string CommandName => "install";

        public async Task RunAsync(ToolsContext context)
        {
            if(context.LatestVersion == null)
            {
                var matchingVersions = context.Versions.OrderBy(a => a.GetNumber());
                if(string.IsNullOrEmpty(context.VersionNumber))
                {
                    Console.WriteLine("Install latest versions: " + matchingVersions.Last().VersionNumber);
                    var portalVersions = InstallingVersion(matchingVersions, context);
                    foreach(var portalVersion in portalVersions)
                    {
                        await context.VersionRepository.AddAsync(portalVersion);
                    }
                }
                else
                {
                    var requestVersionNumber = context.VersionNumber.ToVersionNumber();
                    matchingVersions = matchingVersions.Where(a => a.VersionNumber.ToVersionNumber() <= requestVersionNumber).OrderBy(b => b.GetNumber());
                    var portalVersions = InstallingVersion(matchingVersions, context);
                    foreach(var portalVersion in portalVersions)
                    {
                        await context.VersionRepository.AddAsync(portalVersion);
                    }
                }
            }
            else
            {
                Console.WriteLine("Sorry we can't install any version because it is already exist.");
            }
        }

        private List<Version> InstallingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<Version>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            foreach(var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Installing Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach(var version in matchingVersions)
                {
                    var executionName = version.GetType().GetTypeInfo().Name;
                    version.Upgrade(toolsContext.VersionContext);
                    executingVersions.Add(executionName);
                    Console.WriteLine(string.Format("Installing {0} Version {1} Completely!", executionName, version.VersionNumber));
                }

                dicVersions.Add(groupVersion, executingVersions);
            }

            foreach(var kvp in dicVersions)
            {
                effectivePortalVersions.Add(new Version
                {
                    Id = DataUtil.GenerateUniqueId(),
                    VersionNumber = kvp.Key,
                    AffectiveList = ConvertUtil.SerializeObject(kvp.Value),
                    CreatedDate = DateTime.UtcNow
                });
            }

            return effectivePortalVersions;
        }
    }
}
