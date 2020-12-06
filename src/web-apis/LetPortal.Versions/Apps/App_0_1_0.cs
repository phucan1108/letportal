using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;

namespace LetPortal.Versions.Apps
{
    public class App_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<App>(Constants.CoreAppId);

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var coreApp = new App
            {
                Id = Constants.CoreAppId,
                Name = "core-app",
                Logo = "important_devices",
                Author = "Admin",
                CreatedDate = DateTime.UtcNow,
                DefaultUrl = "/portal/page/pages-management",
                CurrentVersionNumber = "0.0.1",
                DisplayName = "Core",
                ModifiedDate = DateTime.UtcNow,
                Menus = new List<Menu>
                {
                    new Menu
                    {
                        Id = "5cf616bc462b56ee3bc2c7e1",
                        DisplayName = "Core",
                        Icon = "settings",
                        Url = "#",
                        MenuPath = "~",
                        Order = 0,
                        SubMenus = new List<Menu>
                        {
                            new Menu
                            {
                                Id = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                                DisplayName = "Apps Management",
                                Icon = "camera_enhance",
                                Url = "/portal/page/apps-management",
                                Order = 0,
                                ParentId = "5cf616bc462b56ee3bc2c7e1",
                                MenuPath = "~/5cf616bc462b56ee3bc2c7e1"
                            },
                            new Menu
                            {
                                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                                DisplayName = "Databases Management",
                                Icon = "view_module",
                                Url = "/portal/page/databases-management",
                                Order = 1,
                                ParentId = "5cf616bc462b56ee3bc2c7e1",
                                MenuPath = "~/5cf616bc462b56ee3bc2c7e1"
                            },
                            new Menu
                            {
                                Id = "5e1aa91e3c107562acf358b3",
                                DisplayName = "Backup Management",
                                Icon = "backup",
                                Url = "/portal/page/backup-management",
                                Order = 1,
                                ParentId = "5cf616bc462b56ee3bc2c7e1",
                                MenuPath = "~/5cf616bc462b56ee3bc2c7e1"
                            }
                        }
                    },
                    new Menu
                    {
                        Id = "5d4d8adfae5f5b68b811ec24",
                        DisplayName = "Page Settings",
                        Icon = "pages",
                        Url = "#",
                        MenuPath = "~",
                        Order = 0,
                        SubMenus = new List<Menu>
                        {
                            new Menu
                            {
                                Id = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                                DisplayName = "Standards Management",
                                Icon = "category",
                                Url = "/portal/page/standard-list-management",
                                Order = 0,
                                ParentId = "5d4d8adfae5f5b68b811ec24",
                                MenuPath = "~/5d4d8adfae5f5b68b811ec24"
                            },
                            new Menu
                            {
                                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                                DisplayName = "Dynamic List Management",
                                Icon = "view_module",
                                Url = "/portal/page/dynamic-list-management",
                                Order = 1,
                                ParentId = "5d4d8adfae5f5b68b811ec24",
                                MenuPath = "~/5d4d8adfae5f5b68b811ec24"
                            },
                            new Menu
                            {
                                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24c2",
                                DisplayName = "Charts Management",
                                Icon = "bar_chart",
                                Url = "/portal/page/charts-management",
                                Order = 2,
                                ParentId = "5d4d8adfae5f5b68b811ec24",
                                MenuPath = "~/5d4d8adfae5f5b68b811ec24"
                            },
                            new Menu
                            {
                                Id = "5cf618cd9ec1d3bf5c339614",
                                DisplayName = "Pages Management",
                                Icon = "receipt",
                                Url = "/portal/page/pages-management",
                                Order = 3,
                                ParentId = "5d4d8adfae5f5b68b811ec24",
                                MenuPath = "~/5d4d8adfae5f5b68b811ec24"
                            }
                        }
                    } ,
                    new Menu
                    {
                        Id = "5cf61767db0f4125341873c1",
                        DisplayName = "Identity",
                        Icon = "verified_user",
                        Url = "#",
                        MenuPath = "~",
                        Order = 1,
                        SubMenus= new List<Menu>
                        {
                            new Menu
                            {
                                Id = "5cf61827f2a8d9aa14fab275",
                                DisplayName = "Users Management",
                                Icon = "assignment_ind",
                                Url = "/portal/page/users-management",
                                Order = 0,
                                ParentId = "5cf61767db0f4125341873c1",
                                MenuPath = "~/5cf61767db0f4125341873c1"
                            },
                            new Menu
                            {
                                Id = "5cf6188388edcca91ef04879",
                                DisplayName = "Roles Management",
                                Icon = "card_membership",
                                Url = "/portal/page/roles-management",
                                Order = 1,
                                ParentId = "5cf61767db0f4125341873c1",
                                MenuPath = "~/5cf61767db0f4125341873c1"
                            }
                        }
                    } ,
                    new Menu
                    {
                        Id = "5dcac739be0b4e533408344b",
                        DisplayName = "Services",
                        Icon = "memory",
                        Url = "#",
                        MenuPath = "~",
                        Order = 1,
                        SubMenus= new List<Menu>
                        {
                            new Menu
                            {
                                Id = "5cf61827f2a8d9aa14fab275",
                                DisplayName = "Services Monitor",
                                Icon = "system_update_alt",
                                Url = "/portal/page/services-monitor",
                                Order = 0,
                                ParentId = "5dcac739be0b4e533408344b",
                                MenuPath = "~/5dcac739be0b4e533408344b"
                            },
                            new Menu
                            {
                                Id = "5cf6188388edcca91ef04879",
                                DisplayName = "Service Logs",
                                Icon = "event_note",
                                Url = "/portal/page/service-logs",
                                Order = 1,
                                ParentId = "5dcac739be0b4e533408344b",
                                MenuPath = "~/5dcac739be0b4e533408344b"
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(coreApp);
            return Task.CompletedTask;
        }
    }
}
