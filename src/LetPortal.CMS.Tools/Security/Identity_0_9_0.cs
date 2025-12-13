using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Identity;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Repositories.Identity;

namespace LetPortal.CMS.Tools.Security
{
    public class Identity_0_9_0 : IIdentityVersion
    {
        private readonly IUserRepository _userRepository;

        public Identity_0_9_0(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string VersionNumber => "0.9.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Role>("5f6074fbf7a6d360d8a908e7");
            return Task.CompletedTask;
        }

        public async Task Upgrade(IVersionContext versionContext)
        {
            var siteAdminRole = new Role
            {
                Id = "5f6074fbf7a6d360d8a908e7",
                Name = Roles.SiteAdmin,
                DisplayName = "Site Admin",
                NormalizedName = "SITEADMIN",
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp(Constants.CMS_APP_ID)
                }
            };

            var superAdmin = await _userRepository.FindByNormalizedUsername("admin".ToUpper());

            superAdmin.Roles.Add(Roles.SiteAdmin);
            await _userRepository.UpdateAsync(superAdmin.Id, superAdmin);

            versionContext.InsertData(siteAdminRole);
        }
    }
}
