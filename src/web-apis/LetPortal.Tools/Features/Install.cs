using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Tools;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Versions.Patches;
using Version = LetPortal.Core.Versions.Version;

namespace LetPortal.Tools.Features
{
    class Install : IFeatureCommand
    {
        public string CommandName => "install";

        public async Task RunAsync(object context)
        {
            var toolsContext = context as ToolsContext;
            if (string.IsNullOrEmpty(toolsContext.Arguments.PatchName))
            {
                await InstallWithApp(toolsContext);
            }
            else
            {
                await InstallWithPatch(toolsContext);
            }
        }

        private async Task InstallWithPatch(ToolsContext toolsContext)
        {
            if (toolsContext.LatestPatchVersion == null)
            {
                if(toolsContext.Arguments.App == "portal")
                {
                    var patchFeature = toolsContext.CurrentPatchPortal;
                    var matchingVersions = patchFeature.GetAllVersions(toolsContext.Services).OrderBy(a => a.VersionNumber.ToVersionNumber());
                    if (string.IsNullOrEmpty(toolsContext.VersionNumber))
                    {
                        Console.WriteLine("-----------------------INSTALL PATCH ---------------------------");
                        Console.WriteLine("PATCH NAME: " + patchFeature.PatchName);
                        Console.WriteLine("INSTALLING VERSION: " + matchingVersions.Last().VersionNumber);
                        Console.WriteLine("-----------------------++++++++++++++++-------------------------");
                        var portalVersions = await InstallingPatchVersion(matchingVersions, toolsContext, true);
                        foreach (var portalVersion in portalVersions)
                        {
                            await toolsContext.PatchVersionRepository.AddAsync(portalVersion);
                        }
                    }
                    else
                    {
                        var requestVersionNumber = toolsContext.VersionNumber.ToVersionNumber();
                        matchingVersions = matchingVersions.Where(a => a.VersionNumber.ToVersionNumber() <= requestVersionNumber).OrderBy(b => b.GetNumber());
                        var portalVersions = await InstallingPatchVersion(matchingVersions, toolsContext, true);
                        foreach (var portalVersion in portalVersions)
                        {
                            await toolsContext.PatchVersionRepository.AddAsync(portalVersion);
                        }
                    }
                }

                if (toolsContext.Arguments.App == "identity")
                {
                    var patchFeature = toolsContext.CurrentIdentityPortal;
                    var matchingVersions = patchFeature.GetAllVersions(toolsContext.Services).OrderBy(a => a.VersionNumber.ToVersionNumber());
                    if (string.IsNullOrEmpty(toolsContext.VersionNumber))
                    {
                        Console.WriteLine("-----------------------INSTALL PATCH ---------------------------");
                        Console.WriteLine("PATCH NAME: " + patchFeature.PatchName);
                        Console.WriteLine("INSTALLING VERSION: " + matchingVersions.Last().VersionNumber);
                        Console.WriteLine("-----------------------++++++++++++++++-------------------------");
                        var portalVersions = await InstallingPatchVersion(matchingVersions, toolsContext, false);
                        foreach (var portalVersion in portalVersions)
                        {
                            await toolsContext.PatchVersionRepository.AddAsync(portalVersion);
                        }
                    }
                    else
                    {
                        var requestVersionNumber = toolsContext.VersionNumber.ToVersionNumber();
                        matchingVersions = matchingVersions.Where(a => a.VersionNumber.ToVersionNumber() <= requestVersionNumber).OrderBy(b => b.GetNumber());
                        var portalVersions = await InstallingPatchVersion(matchingVersions, toolsContext, false);
                        foreach (var portalVersion in portalVersions)
                        {
                            await toolsContext.PatchVersionRepository.AddAsync(portalVersion);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Sorry we can't install any version because it is already exist.");
            }
        }
        private async Task<List<PatchVersion>> InstallingPatchVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext, bool isPortal)
        {
            var effectivePortalVersions = new List<PatchVersion>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            IPatchProcessor patchProcessor = new PatchProcessor();
            foreach (var groupVersion in availableGroupVersions)
            {
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine($"------------------Installing Version: {groupVersion}---------------------");
                Console.WriteLine("----------------------------------------------------------------");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach (var version in matchingVersions)
                {
                    var executionName = version.GetType().GetTypeInfo().Name;
                    await version.Upgrade(toolsContext.VersionContext);
                    executingVersions.Add(executionName);
                    Console.WriteLine(string.Format("Installing {0} Version {1} Completely!", executionName, version.VersionNumber));
                }

                dicVersions.Add(groupVersion, executingVersions);
            }
            foreach (var kvp in dicVersions)
            {
                effectivePortalVersions.Add(new PatchVersion
                {
                    Id = DataUtil.GenerateUniqueId(),
                    AppName = toolsContext.Arguments.App,
                    PatchName = isPortal ? toolsContext.CurrentPatchPortal.PatchName : toolsContext.CurrentIdentityPortal.PatchName,
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    CreatedDate = DateTime.UtcNow
                });
            }

            return effectivePortalVersions;
        }
        private async Task InstallWithApp(ToolsContext toolsContext)
        {
            if (toolsContext.LatestVersion == null)
            {
                var matchingVersions = toolsContext.Versions.OrderBy(a => a.GetNumber());
                if (string.IsNullOrEmpty(toolsContext.VersionNumber))
                {
                    Console.WriteLine("-----------------------INSTALL PROGRESS-------------------------");
                    Console.WriteLine("INSTALLING VERSION: " + matchingVersions.Last().VersionNumber);
                    Console.WriteLine("-----------------------++++++++++++++++-------------------------");
                    var portalVersions = await InstallingVersion(matchingVersions, toolsContext);
                    foreach (var portalVersion in portalVersions)
                    {
                        await toolsContext.VersionRepository.AddAsync(portalVersion);
                    }
                }
                else
                {
                    var requestVersionNumber = toolsContext.VersionNumber.ToVersionNumber();
                    matchingVersions = matchingVersions.Where(a => a.VersionNumber.ToVersionNumber() <= requestVersionNumber).OrderBy(b => b.GetNumber());
                    var portalVersions = await InstallingVersion(matchingVersions, toolsContext);
                    foreach (var portalVersion in portalVersions)
                    {
                        await toolsContext.VersionRepository.AddAsync(portalVersion);
                    }
                }
            }
            else
            {
                Console.WriteLine("Sorry we can't install any version because it is already exist.");
            }
        }

        private async Task<List<Version>> InstallingVersion(IEnumerable<IVersion> versions, ToolsContext toolsContext)
        {
            var effectivePortalVersions = new List<Version>();
            var dicVersions = new Dictionary<string, List<string>>();

            var availableGroupVersions = versions.Select(a => a.VersionNumber).Distinct();

            IPatchProcessor patchProcessor = new PatchProcessor();
            foreach (var groupVersion in availableGroupVersions)
            {
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine($"------------------Installing Version: {groupVersion}---------------------");
                Console.WriteLine("----------------------------------------------------------------");
                var matchingVersions = versions.Where(a => a.VersionNumber == groupVersion);

                var executingVersions = new List<string>();
                foreach (var version in matchingVersions)
                {
                    var executionName = version.GetType().GetTypeInfo().Name;
                    await version.Upgrade(toolsContext.VersionContext);
                    executingVersions.Add(executionName);
                    Console.WriteLine(string.Format("Installing {0} Version {1} Completely!", executionName, version.VersionNumber));
                }

                if (toolsContext.AllowPatch)
                {
                    var result = await patchProcessor.Proceed(Path.Combine(toolsContext.PatchesFolder, groupVersion), toolsContext.VersionContext.DatabaseOptions as DatabaseOptions);
                    if (result.Any())
                    {
                        foreach (var file in result)
                        {
                            Console.WriteLine(string.Format("Patched file: {0}", file));
                        }
                    }
                }

                dicVersions.Add(groupVersion, executingVersions);
            }
            foreach (var kvp in dicVersions)
            {
                effectivePortalVersions.Add(new Version
                {
                    Id = DataUtil.GenerateUniqueId(),
                    AppName = toolsContext.Arguments.App,
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    CreatedDate = DateTime.UtcNow
                });
            }

            return effectivePortalVersions;
        }
    }
}
