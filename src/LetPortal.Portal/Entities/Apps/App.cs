using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.Menus;

namespace LetPortal.Portal.Entities.Apps
{
    [EntityCollection(Name = "apps")]
    [Table("apps")]
    public class App : BackupableEntity , ICodeGenerable
    {
        [StringLength(250)]
        public string Logo { get; set; }

        [StringLength(250)]
        public string DefaultUrl { get; set; }

        [StringLength(250)]
        public string Author { get; set; }

        [StringLength(50)]
        public string CurrentVersionNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<Menu> Menus { get; set; } = new List<Menu>();

        public List<MenuProfile> MenuProfiles { get; set; } = new List<MenuProfile>();

        public void AddSubMenu(string parentMenu, Menu subMenu)
        {
            var menu = Menus.First(a => a.Id == parentMenu);
            menu.SubMenus.Add(subMenu);
        }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.Apps.App>(\"{Id}\");"
            };
            var stringBuilder = new StringBuilder();

            varName = varName != null ? varName: Name.Replace("-", "", StringComparison.OrdinalIgnoreCase) + "App";
            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.Apps.App");
            _ = stringBuilder.AppendLine("{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    Logo = \"{Logo}\",");
            _ = stringBuilder.AppendLine($"    Author = \"{Author}\",");
            _ = stringBuilder.AppendLine($"    DefaultUrl = \"{DefaultUrl}\",");
            _ = stringBuilder.AppendLine($"    CurrentVersionNumber = \"{CurrentVersionNumber}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");            
            _ = stringBuilder.AppendLine($"    CreatedDate = DateTime.Now,");
            _ = stringBuilder.AppendLine($"    ModifiedDate = DateTime.Now,");
            if(Menus != null && Menus.Any())
            {
                _ = stringBuilder.AppendLine($"    Menus = new List<LetPortal.Portal.Entities.Menus.Menu>");
                _ = stringBuilder.AppendLine("    {");
                foreach(var menu in Menus)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Menus.Menu");
                    _ = stringBuilder.AppendLine("        {");
                    _ = stringBuilder.AppendLine($"            Id = \"{menu.Id}\",");
                    _ = stringBuilder.AppendLine($"            DisplayName = \"{menu.DisplayName}\",");
                    _ = stringBuilder.AppendLine($"            Icon = \"{menu.Icon}\",");
                    _ = stringBuilder.AppendLine($"            Url = \"{menu.Url}\",");
                    _ = stringBuilder.AppendLine($"            MenuPath = \"{menu.MenuPath}\",");
                    _ = stringBuilder.AppendLine($"            Order = {menu.Order},");
                    if(menu.SubMenus != null && menu.SubMenus.Any())
                    {
                        _ = stringBuilder.AppendLine($"            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>");
                        _ = stringBuilder.AppendLine("            {");
                        foreach(var subMenu in menu.SubMenus)
                        {
                            _ = stringBuilder.AppendLine($"            new LetPortal.Portal.Entities.Menus.Menu");
                            _ = stringBuilder.AppendLine("            {");
                            _ = stringBuilder.AppendLine($"                Id = \"{subMenu.Id}\",");
                            _ = stringBuilder.AppendLine($"                DisplayName = \"{subMenu.DisplayName}\",");
                            _ = stringBuilder.AppendLine($"                Icon = \"{subMenu.Icon}\",");
                            _ = stringBuilder.AppendLine($"                Url = \"{subMenu.Url}\",");
                            _ = stringBuilder.AppendLine($"                MenuPath = \"{subMenu.MenuPath}\",");
                            _ = stringBuilder.AppendLine($"                Order = {subMenu.Order},");
                            _ = stringBuilder.AppendLine("             },");
                        }
                        _ = stringBuilder.AppendLine("            }");
                    }
                    _ = stringBuilder.AppendLine("        },");
                }
                _ = stringBuilder.AppendLine("    },");
            }

            if(MenuProfiles != null && MenuProfiles.Any())
            {
                _ = stringBuilder.AppendLine($"    MenuProfiles = new List<LetPortal.Portal.Entities.Apps.MenuProfile>");
                _ = stringBuilder.AppendLine("    {");
                foreach(var menuProfile in MenuProfiles)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Apps.MenuProfile");
                    _ = stringBuilder.AppendLine("        {");
                    _ = stringBuilder.AppendLine($"            Role = \"{menuProfile.Role}\",");
                    _ = stringBuilder.AppendLine($"            MenuIds = new List<string>");
                    _ = stringBuilder.AppendLine("            {");
                    foreach(var menuId in menuProfile.MenuIds)
                    {
                        _ = stringBuilder.AppendLine($"                 \"{menuId}\",");
                    }
                    _ = stringBuilder.AppendLine("            }");
                    _ = stringBuilder.AppendLine("        },");
                }
                _ = stringBuilder.AppendLine("    }");
            }
            _ = stringBuilder.AppendLine("};");
            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class MenuProfile
    {
        public string Role { get; set; }

        public List<string> MenuIds { get; set; }
    }

    [EntityCollection(Name = "appversions")]
    public class AppVersion : Entity
    {
        public string AppId { get; set; }

        public string VersionNumber { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public string HashKey { get; set; }

        public string CompressionData { get; set; }
    }
}
