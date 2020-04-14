using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Versions;
using LetPortal.Identity.Entities;
using LetPortal.Portal.Constants;

namespace LetPortal.Versions.Identity
{
    public class Identity_0_5_2 : IIdentityVersion
    {
        public string VersionNumber => "0.5.2";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Role>("5e6b506e52605e513cd02265");
            versionContext.DeleteData<Role>("5e6b506e52605e513cd02266");
            versionContext.DeleteData<Role>("5e6b506e52605e513cd02267");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var adminRole = new Role
            {
                Id = "5e6b506e52605e513cd02265",
                Name = RolesConstants.ADMIN,
                NormalizedName = RolesConstants.ADMIN.ToUpper(System.Globalization.CultureInfo.CurrentCulture),
                DisplayName = RolesConstants.ADMIN,
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp(Constants.CoreAppId)
                }
            };

            adminRole.Claims.AddRange(StandardClaims
                .GenerateClaimsByPages(new string[]
                {
                     "apps-management",
                     "databases-management",
                     "pages-management",
                     "page-builder",
                     "menus",
                     "role-claims",
                     "dynamic-list-builder",
                     "roles-management",
                     "users-management",
                     "database-form",
                     "app-form",
                     "dynamic-list-management",
                     "standard-list-management",
                     "role-form",
                     "add-user-form",
                     "user-form",
                     "charts-management",
                     "services-monitor",
                     "service-logs",
                     "service-dashboard",
                     "backup-management",
                     "chart-builder",
                     "backup-builder",
                     "backup-upload",
                     "backup-restore",
                     "user-info"
                }));

            var developerRole = new Role
            {
                Id = "5e6b506e52605e513cd02266",
                Name = RolesConstants.DEVELOPER,
                NormalizedName = RolesConstants.DEVELOPER.ToUpper(System.Globalization.CultureInfo.CurrentCulture),
                DisplayName = RolesConstants.DEVELOPER,
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp(Constants.CoreAppId)
                }
            };

            developerRole.Claims.AddRange(StandardClaims
                .GenerateClaimsByPages(new string[]
                {
                     "pages-management",
                     "page-builder",
                     "dynamic-list-builder",
                     "dynamic-list-management",
                     "standard-list-management",
                     "charts-management",
                     "services-monitor",
                     "service-logs",
                     "service-dashboard",
                     "chart-builder",
                     "user-info"
                }));

            var userRole = new Role
            {
                Id = "5e6b506e52605e513cd02267",
                Name = RolesConstants.USER,
                NormalizedName = RolesConstants.USER.ToUpper(System.Globalization.CultureInfo.CurrentCulture),
                DisplayName = RolesConstants.USER,
                Claims = new List<BaseClaim>
                {

                }
            };

            userRole.Claims.AddRange(StandardClaims
                .GenerateClaimsByPages(new string[]
                {
                     "user-info"
                }));
            versionContext.InsertData(adminRole);
            versionContext.InsertData(developerRole);
            versionContext.InsertData(userRole);
        }
    }
}
