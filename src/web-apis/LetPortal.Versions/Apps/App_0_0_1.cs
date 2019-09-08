using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using System;
using System.Collections.Generic;

namespace LetPortal.Versions.Apps
{
    public class App_0_0_1 : IVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<App>(Constants.CoreAppId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var coreApp = new App
            {
                Id = Constants.CoreAppId,
                Name = "core-app",
                Logo = "important_devices",
                Author = "Admin",
                DateCreated = DateTime.UtcNow,
                DefaultUrl = "/portal/page/apps-management",
                CurrentVersionNumber = "0.0.1",
                DisplayName = "Core",
                DateModified = DateTime.UtcNow,
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
                                MenuPath = "~/5cf616bc462b56ee3bc2c7e1",
                                SubMenus = new List<Portal.Entities.Menus.Menu>
                                {
                                    new Portal.Entities.Menus.Menu
                                    {
                                        Id = "3a035505-871a-9c26-41a3-9314ee5c080d",
                                        DisplayName = "App Form",
                                        Url = "portal/page/app-form",
                                        Hide = true,
                                        ParentId = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                                        MenuPath = "~/5cf616bc462b56ee3bc2c7e1/cc0efc5d-f80a-f33e-35e2-5d0c12b818c5"
                                    }
                                }
                            },
                            new Menu
                            {
                                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                                DisplayName = "Databases Management",
                                Icon = "view_module",
                                Url = "/portal/page/databases-management",
                                Order = 1,
                                ParentId = "5cf616bc462b56ee3bc2c7e1",
                                MenuPath = "~/5cf616bc462b56ee3bc2c7e1",
                                SubMenus = new List<Portal.Entities.Menus.Menu>
                                {
                                    new Portal.Entities.Menus.Menu
                                    {
                                        Id = "578f1999-6d82-6d22-596d-b94827ded210",
                                        DisplayName = "Database Form",
                                        Url = "portal/page/database-form",
                                        Order = 0,
                                        Hide = true,
                                        ParentId = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                                        MenuPath = "~/5cf616bc462b56ee3bc2c7e1/172fd9c3-0168-90d1-5de7-60b0acaa24ea"
                                    }
                                }
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
                    }
                }
            };

            versionContext.InsertData(coreApp);
        }
    }
}
