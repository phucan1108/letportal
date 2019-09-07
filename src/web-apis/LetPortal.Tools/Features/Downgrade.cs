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
    class Downgrade : IFeatureCommand
    {
        public string CommandName => "down";

        public async Task RunAsync(ToolsContext context)
        {
            if(context.LatestVersion != null)
            {
                var requestingVersionNumber = context.VersionNumber.ToVersionNumber();
                if(requestingVersionNumber < context.LatestVersion.GetNumber())
                {
                    var matchingVersions = context.Versions.Where(
                         a => a.GetNumber() <= context.LatestVersion.GetNumber()
                             && a.GetNumber() > requestingVersionNumber);

                    var portalVersions = DowngradingVersion(matchingVersions, context);                    
                    foreach(var portalVersion in portalVersions)
                    {
                        var storedVersion = context.PortalVersionRepository.GetAsQueryable().Where(a => a.VersionNumber == portalVersion.VersionNumber).FirstOrDefault();

                        if(storedVersion != null)
                        {
                            await context.PortalVersionRepository.DeleteAsync(storedVersion.Id);
                        }                        
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Oops we can't downgrade version because your request is {0} but the current is {1}", context.VersionNumber, context.LatestVersion.VersionNumber));
                }
            }
            else
            {
                Console.WriteLine("Oops we can't find any installation in the database, please use 'install' command before downgrading.");
            }
        }

        private List<PortalVersion> DowngradingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<PortalVersion>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.OrderByDescending(b => b.GetNumber()).Select(a => a.VersionNumber).Distinct();

            foreach(var groupVersion in availableGroupVersions)
            {
                Console.WriteLine($"Downgrading Version: {groupVersion}");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach(var version in matchingVersions)
                {
                    version.Downgrade(toolsContext.VersionContext);
                    executingVersions.Add(version.GetType().GetTypeInfo().Name);
                    Console.WriteLine(string.Format("Downgrading {0} Version {1} Completely!", version.GetType().GetTypeInfo().Name, version.VersionNumber));
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
                    CreatedDate = DateTime.UtcNow
                });
            }

            return effectivePortalVersions;
        }
    }
}
