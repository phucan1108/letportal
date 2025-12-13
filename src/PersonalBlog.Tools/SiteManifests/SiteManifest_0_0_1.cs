using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.SiteManifests
{
    public class SiteManifest_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var constructionManifests = new List<Tuple<string, string, ManifestEditorType>>
            {
                new Tuple<string, string, ManifestEditorType>("SiteName","Personal Blog", ManifestEditorType.Textbox),
                new Tuple<string, string, ManifestEditorType>("CompanyName","Let Portal", ManifestEditorType.Textbox)
            };

            var siteManifests = constructionManifests.Select(a => Convert(a));

            foreach (var manifest in siteManifests)
            {
                versionContext.InsertData(manifest);
            }
            return Task.CompletedTask;
        }

        private SiteManifest Convert(Tuple<string, string, ManifestEditorType> tuple)
        {
            return new SiteManifest
            {
                Id = DataUtil.GenerateUniqueId(),
                SiteId = "5f01f1a4b41be2672499fcd3",
                ConfigurableValue = tuple.Item2,
                EditorType = tuple.Item3,
                Key = tuple.Item1
            };
        }
    }
}
