using System;
using System.Collections.Generic;
using LetPortal.Core.Versions;
using LetPortal.Identity.Entities;

namespace LetPortal.Versions.Identity
{
    public class Identity_0_1_0 : IIdentityVersion
    {
        public string VersionNumber => "0.1.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Role>("5c06a15e4cc9a850bca44488");
            versionContext.DeleteData<User>("5ce287ee569d6f23e8504cef");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var superAdminRole = new Role
            {
                Id = "5c06a15e4cc9a850bca44488",
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                DisplayName = "Super Admin",
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp(Constants.CoreAppId)
                }
            };

            // Pass: @Dm1n!
            var adminAccount = new User
            {
                Id = "5ce287ee569d6f23e8504cef",
                Username = "admin",
                NormalizedUserName = "ADMIN",
                Domain = string.Empty,
                PasswordHash = "AQAAAAEAACcQAAAAEBhhMYTL5kwYqXheHSdarA/+vleSI07yGkTKNw1bb1jrTlYnBZK1CZ+zdHnqWwLLDA==",
                Email = "admin@portal.com",
                NormalizedEmail = "ADMIN@PORTAL.COM",
                IsConfirmedEmail = true,
                SecurityStamp = "7YHYVBYWLTYC4EAPVRS2SWX2IIUOZ3XM",
                AccessFailedCount = 0,
                IsLockoutEnabled = false,
                LockoutEndDate = DateTime.UtcNow,
                Roles = new List<string>
                {
                    "SuperAdmin"
                },
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessAppSelectorPage,
                    StandardClaims.Sub("5ce287ee569d6f23e8504cef"),
                    StandardClaims.UserId("5ce287ee569d6f23e8504cef"),
                    StandardClaims.Name("admin")
                }
            };

            versionContext.InsertData(adminAccount);
            versionContext.InsertData(superAdminRole);
        }
    }
}
