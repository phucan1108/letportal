using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace LetPortal.CMS.Tools
{
    public class CMS_0_1_0 : LetPortal.Portal.IPortalVersion
    {
        public string VersionNumber => "0.1.0";
        public Task Downgrade(LetPortal.Core.Versions.IVersionContext versionContext)
        {
            versionContext.DeleteData<LetPortal.Portal.Entities.Apps.App>("5f23f7d6b8f393672ce21029");
            versionContext.DeleteData<LetPortal.Portal.Entities.Databases.DatabaseConnection>("5f33c1ddedf7b3de91e106d8");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5f47ede309012d61101e3416");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5f49fc17dce086279c4aa461");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5f527543775f3a5ee4b2ede7");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5f527a1f775f3a5ee4b2ef01");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5fb69456ea2c730001a775ad");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5fb8a918978a6b0001a0254c");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5fb8ee954f013c00017fdaae");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5fb6b4104f46d90001e67e8b");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5fb761050383a10001ba17c2");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f469621c0a5165a80016f71");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f47d2e709012d61101e3162");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f47e4ed09012d61101e323f");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f4e745afc4f674a9c1aa50a");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f4f310ec9957252a4103950");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5fb639108e67fa00011797dd");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5fb8a530978a6b0001a024b9");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5fb8e9574f013c00017fd9f6");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e21e09012d61101e31bb");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e27609012d61101e31d8");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e56d09012d61101e3272");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47eeb909012d61101e343e");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f4e75b9fc4f674a9c1aa57e");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f4f3148c9957252a4103999");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f5276b4775f3a5ee4b2ee14");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb60b698e67fa00011791a7");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb6392a8e67fa00011797e0");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb69663ea2c730001a775f6");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb8a61f978a6b0001a024da");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb8aad7978a6b0001a0258b");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb8e9ed4f013c00017fda0d");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fb8f0114f013c00017fdae4");
            versionContext.DeleteData<LetPortal.Portal.Entities.Components.Controls.CompositeControl>("5fcbb5aee19fcf0001a2d90d");
            return System.Threading.Tasks.Task.CompletedTask;
        }
        public Task Upgrade(LetPortal.Core.Versions.IVersionContext versionContext)
        {
            var cmsappApp = new LetPortal.Portal.Entities.Apps.App
            {
                Id = "5f23f7d6b8f393672ce21029",
                Name = "cms-app",
                Logo = "donut_small",
                Author = "Admin",
                DefaultUrl = "/portal/page/cms-sites",
                CurrentVersionNumber = "0.1.0",
                DisplayName = "CMS",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Menus = new List<LetPortal.Portal.Entities.Menus.Menu>
    {
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5f338e839bf207e7c836b652",
            DisplayName = "Create",
            Icon = "add",
            Url = "#",
            MenuPath = "~",
            Order = 0,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5f33bd4add8e42a9f7fc6d91",
                DisplayName = "Site",
                Icon = "home_work",
                Url = "/portal/page/cms-site",
                MenuPath = "~/5f338e839bf207e7c836b652",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "685f8c2d-866d-a6fa-0241-aebd58ea45a9",
                DisplayName = "Page",
                Icon = "account_balance_wallet",
                Url = "portal/page/cms-page",
                MenuPath = "~/5f338e839bf207e7c836b652",
                Order = 1,
             },
            }
        },
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5f258a65807fec5a88e22c2e",
            DisplayName = "Content",
            Icon = "toys",
            Url = "#",
            MenuPath = "~",
            Order = 0,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                DisplayName = "Sites",
                Icon = "cloud_circle",
                Url = "/portal/page/cms-sites",
                MenuPath = "~/5f258a65807fec5a88e22c2e",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "",
                DisplayName = "Page Templates",
                Icon = "airplay",
                Url = "portal/page/cms-page-templates",
                MenuPath = "",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5e1aa91e3c107562acf358b3",
                DisplayName = "Themes",
                Icon = "backup_table",
                Url = "/portal/page/cms-themes",
                MenuPath = "~/5f258a65807fec5a88e22c2e",
                Order = 1,
             },
            }
        },
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "",
            DisplayName = "Features",
            Icon = "star",
            Url = "#",
            MenuPath = "",
            Order = 0,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "",
                DisplayName = "Blogs",
                Icon = "description",
                Url = "portal/page/cms-blogs",
                MenuPath = "",
                Order = 0,
             },
            }
        },
    },
            };
            versionContext.InsertData(cmsappApp);
            var cmsdbConnection = new LetPortal.Portal.Entities.Databases.DatabaseConnection
            {
                Id = "5f33c1ddedf7b3de91e106d8",
                Name = "cmsdb",
                DisplayName = "CMS Database",
                ConnectionString = "mongodb://mongodb:27017/cms",
                DatabaseConnectionType = "mongodb",
                DataSource = "cms",
            };
            versionContext.InsertData(cmsdbConnection);
            var cmssiteformStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5f47ede309012d61101e3416",
                Name = "cmssiteform",
                DisplayName = "CMS Site",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "_id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter _id",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "true",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "_id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Name",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Name",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "name",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "domains",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Domains",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "domains",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "defaultLocaleId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Default Locale Id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Default Locale Id",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "defaultLocaleId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "enable",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Enable",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Enable",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "enable",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "defaultPathWhenNotFound",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 5,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Default Path When Not Found",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Default Path When Not Found",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "defaultPathWhenNotFound",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "themeId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 7,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Theme",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Choose one theme",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "themeId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"themes\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        }
    },
            };
            versionContext.InsertData(cmssiteformStandard);

            var cmssitemanifestStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5f49fc17dce086279c4aa461",
                Name = "cmssitemanifest",
                DisplayName = "CMS Site Manifest",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Id",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "siteId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Site",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "true",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "siteId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"sites\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "key",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Key",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Key",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "key",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "configurableValue",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Configurable Value",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Configurable Value",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "configurableValue",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "data.editorType === 0",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "1000",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "configurableValue",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Email,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Configurable Value",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Enter a email",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "configurableValue",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "data.editorType === 2",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "250",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = true,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        }
    },
            };
            versionContext.InsertData(cmssitemanifestStandard);

            var cmspageStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5f527543775f3a5ee4b2ede7",
                Name = "cmspage",
                DisplayName = "CMS Page",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Id",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Name",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Name",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "name",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "isRedirected",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Is Redirected",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Is Redirected",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "isRedirected",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "nextRedirectPage",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Next Redirect Page",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Next Redirect Page",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "nextRedirectPage",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "enable",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Enable",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Enable",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "enable",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "pageTemplateId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 6,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Template",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "pageTemplateId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"pagetemplates\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "chosenPageVersionId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 7,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Current Page Version",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "true",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "chosenPageVersionId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"pageversions\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "siteId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 8,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Site",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "!!queryparams.siteid",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "siteId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "{{queryparams.siteid}}",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"sites\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        }
    },
            };
            versionContext.InsertData(cmspageStandard);

            var cmspagegooglemetadataStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5f527a1f775f3a5ee4b2ef01",
                Name = "cmspagegooglemetadata",
                DisplayName = "CMS Page Google Metadata",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "title",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Title",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Title",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "title",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "description",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Description",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Description",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "description",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "robots",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Robots",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Robots",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "robots",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "allowSiteLinkSearch",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Allow Site Link Search",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Allow Site Link Search",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "allowSiteLinkSearch",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "allowGoogleTranslate",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Allow Google Translate",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Allow Google Translate",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "allowGoogleTranslate",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "allowGoogleRead",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 5,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Allow Google Read",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Allow Google Read",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "allowGoogleRead",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "isAdultPage",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 6,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Is Adult Page",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Is Adult Page",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "isAdultPage",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
        }
    },
            };
            versionContext.InsertData(cmspagegooglemetadataStandard);

            var cmspagetemplateinfoStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5fb69456ea2c730001a775ad",
                Name = "cmspagetemplateinfo",
                DisplayName = "CMS Page Template Info",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Id",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "id_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Name",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Name",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "name",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "name_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "themeId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Theme",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "!!queryparams.pageTemplateId",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "themeId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"themes\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "themeId_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "siteId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Site",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "!!queryparams.pageTemplateId",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "siteId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"sites\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "siteId_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        }
    },
            };
            versionContext.InsertData(cmspagetemplateinfoStandard);

            var cmsblogformStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5fb8a918978a6b0001a0254c",
                Name = "cmsblogform",
                DisplayName = "CMS Blog Form",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "true",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "urlPath",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Url Path",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Url Path",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "urlPath",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "5",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "250",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "urlPath_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "title",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Title",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Title",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "title",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "500",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "title_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "description",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Description",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Description",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "description",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "2000",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "description_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        }
    },
            };
            versionContext.InsertData(cmsblogformStandard);

            var cmspostformStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5fb8ee954f013c00017fdaae",
                Name = "cmspostform",
                DisplayName = "CMS Post Form",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Standard,
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "id",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Id",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Id",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "id",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "id_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "urlPath",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Url Path",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Url Path",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "urlPath",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "5",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "250",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            AsyncValidators = new List<LetPortal.Portal.Entities.Components.Controls.PageControlAsyncValidator>
            {
                new LetPortal.Portal.Entities.Components.Controls.PageControlAsyncValidator
                {
                    ValidatorName = "uniquepath",
                    IsActive = true,
                    ValidatorMessage = "This url has been used by another post",
                    AsyncValidatorOptions = new LetPortal.Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                    {
                        EvaluatedExpression = "response.result === null",
                        DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                        {
                            DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                            Query = "{\"$query\":{\"posts\":[{\"$match\":{\"urlPath\":\"{{data.urlPath}}\"}}]}}",
                        },

                        ValidatorType = LetPortal.Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator,
                    }
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "urlPath_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "screenshotUrl",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Uploader,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Screenshot",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "screenshotUrl",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "allowfileurl",
                    Value = "true",
                    Description = "Allow an uploader set downloadable url back to a field after saving instead of file id. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "saveonchange",
                    Value = "true",
                    Description = "Allow an uploader upload a file after user changes. Default: false",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = false,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.FileMaximumSize,
                    IsActive = true,
                    ValidatorMessage = "File must have size less than {{option}} Mb",
                    ValidatorOption = "1",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.FileExtensions,
                    IsActive = true,
                    ValidatorMessage = "File must have an extension in {{option}}",
                    ValidatorOption = "jpg;jpeg;png;gif",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "screenshotUrl_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "title",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Title",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Title",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "title",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "250",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "title_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "description",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textarea,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Description",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Enter a description",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "description",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "textarearows",
                    Value = "6",
                    Description = "Rows attribute of textarea. Default: 6",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "2000",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "description_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "content",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.RichTextEditor,
            Order = 5,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Content",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Enter Content",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "content",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "content_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "showComment",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 8,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Show Comment",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Show Comment",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "showComment",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "showComment_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "allowUserComment",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 9,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Allow User Comment",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Allow User Comment",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "allowUserComment",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "allowUserComment_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "maximumCommentPerDay",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Number,
            Order = 10,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Maximum Comment Per Day",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Maximum Comment Per Day",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "maximumCommentPerDay",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "maximumCommentPerDay_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "readDuration",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Number,
            Order = 11,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Read Duration",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Read Duration",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "readDuration",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "readDuration_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "blogId",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 13,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Blog",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Choose a blog",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "!!queryparams.postId",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "blogId",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"blogs\":[{\"$project\":{\"name\":\"$title\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "blogId_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "tags",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 11,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Tags",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Choose tag",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "tags",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "true",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
    DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
    {
        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
        Query = "{\"$query\":{\"blogtags\":[{\"$project\":{\"name\":\"$tag\",\"value\":\"$_id\"}}]}}",
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please select one option",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        }
    },
            };
            versionContext.InsertData(cmspostformStandard);

            var cmssitemenuStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5fb6b4104f46d90001e67e8b",
                Name = "cmssitemenu",
                DisplayName = "CMS Site Menu",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Tree,
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "indatastructure",
            Value = "nest",
            Description = "Tree - Defines the input structure as flat|nest. Flat is array data and nest is subset data. Default: nest"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "outdatastructure",
            Value = "nest",
            Description = "Tree - Defines the output structure as flat|nested. Flat is array data and nest is subset data. Default: nest"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "displayname",
            Value = "name",
            Description = "Tree - Defines a display name field which is used to be node name. Default: name"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowgenerateid",
            Value = "true",
            Description = "Tree - Allow to generate GUID for NodeId and NodeParentId. Default: false"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "inchildren",
            Value = "sub",
            Description = "Tree - Property name of children in the nested input data. Default: children"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "outchildren",
            Value = "sub",
            Description = "Tree - Property name of children in the nested output data. Default: children"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "nodeidfield",
            Value = "id",
            Description = "Tree - Property name of node which indicates distinct node. Default: id"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "nodeparentfield",
            Value = "parentId",
            Description = "Tree - Property name of node which indicates parent field. Default: parentId"
        },
    },
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Name",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Name",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "name",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "name_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "url",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Url",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Url",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "url",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "url_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        }
    },
            };
            versionContext.InsertData(cmssitemenuStandard);

            var cmspagetemplatesectionsStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5fb761050383a10001ba17c2",
                Name = "cmspagetemplatesections",
                DisplayName = "CMS Page Template Sections",
                AppId = "5f23f7d6b8f393672ce21029",
                LayoutType = LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                Type = LetPortal.Portal.Entities.SectionParts.StandardType.Array,
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "identityfield",
            Value = "key",
            Description = "Array - Identity field must be used to indicate one element. Default: id"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "namefield",
            Value = "name;bindingType;themePartRef",
            Description = "Array - Names field must be used to display one element, can be multiple by ;. Default: name"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowadjustment",
            Value = "true",
            Description = "Array - Allow user to add or remove on element. Default: false"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowupdateparts",
            Value = "true",
            Description = "Array - Allow to update one changed element instead of removing all and then adding all. Default: false"
        },
    },
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "key",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Key",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Key",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "key",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "key_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Name",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Name",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "name",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "name_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "bindingType",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Binding Type",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "bindingType",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
    DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
    {
        JsonResource = "[{\"name\":\"None\",\"value\":0},{\"name\":\"Object\",\"value\":1},{\"name\":\"Array\",\"value\":2}" +
    ",{\"name\":\"Datasource\",\"value\":3}]"
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "bindingType_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "hide",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Slide,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Hide",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Please enter Hide",
                    Description = "Tooltip will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "true",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "hide",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "hide_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "themePartRef",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 4,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Theme Part Ref",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "themePartRef",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = false,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "themePartRef_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        }
    },
            };
            versionContext.InsertData(cmspagetemplatesectionsStandard);

            var cmssitesList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5f469621c0a5165a80016f71",
                Name = "cms-sites",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Sites",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{ \"$query\": { \"sites\": [ ] } }",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "name",
                DisplayName = "Name",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "enable",
                DisplayName = "Enable",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Slide
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "defaultPathWhenNotFound",
                DisplayName = "Default Path",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = false,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-site?id={{data.id}}"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "editmanifest",
                DisplayName = "Edit Manifest",
                Color = "primary",
                Icon = "burst_mode",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 1,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-site-manifests?siteid={{data.id}}"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "pages",
                DisplayName = "Pages",
                Color = "primary",
                Icon = "assignment",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 2,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-pages?siteid={{data.id}}"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "create",
                DisplayName = "Create",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.OutList,
                Order = 3,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-site"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "menu",
                DisplayName = "Menu",
                Color = "primary",
                Icon = "menu",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 4,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-site-menu?siteId={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmssitesList);

            var cmspagesList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5f47d2e709012d61101e3162",
                Name = "cms-pages",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Pages",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{\"$query\":{\"pages\":[{\"$match\":{\"siteId\":\"{{queryparams.siteid}}\"}}]}}",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "name",
                DisplayName = "Name",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "isRedirected",
                DisplayName = "Is Redirected",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Slide
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "nextRedirectPage",
                DisplayName = "Next Redirect Page",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "enable",
                DisplayName = "Enable",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Slide
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-page?siteid={{queryparams.siteid}}&&id={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmspagesList);

            var cmssitemanifestsList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5f47e4ed09012d61101e323f",
                Name = "cmssitemanifests",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Site Manifests",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{\"$query\":{\"sitemanifests\":[{\"$match\":{\"siteId\":\"{{queryparams.siteid}}\"}}]}}",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "key",
                DisplayName = "Key",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "configurableValue",
                DisplayName = "Configurable Value",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "editorType",
                DisplayName = "Editor Type",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Select
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = @"[{""name"":""Textbox"",""value"":0},{""name"":""Number"",""value"":1},{""name"":""Email"",""value"":2},{""name"":""DatePicker"",""value"":3},{""name"":""RichTextEditor"",""value"":4},{""name"":""MediaEditor"",""value"":5},{""name"":""KeyValueEditor"",""value"":6},{""name"":""LinkEditor"",""value"":7},{""name"":""JsonEditor"",""value"":8}]"
                   },
                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "back",
                DisplayName = "Back",
                Color = "primary",
                Icon = "cance",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.OutList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-sites"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 1,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-site-manifests?siteid={{queryparams.siteid}}&id={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmssitemanifestsList);

            var cmsthemesList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5f4e745afc4f674a9c1aa50a",
                Name = "cmsthemes",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Themes",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{ \"$query\": { \"themes\": [ ] } }",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "name",
                DisplayName = "Name",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "description",
                DisplayName = "Description",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "creator",
                DisplayName = "Creator",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "thememanifests",
                DisplayName = "Theme Manifests",
                Color = "primary",
                Icon = "art_track",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-theme-manifests?id={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmsthemesList);

            var cmsthememanifestsList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5f4f310ec9957252a4103950",
                Name = "cmsthememanifests",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Theme Manifests",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{\"$query\":{\"themes\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.id}}\')\"}},{\"$unwin" +
                "d\":{\"path\":\"$themeManifests\"}},{\"$replaceRoot\":{\"newRoot\":\"$themeManifests\"}}]}}" +
                "",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "key",
                DisplayName = "Key",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "editorType",
                DisplayName = "Editor Type",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Select
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = @"[{""name"":""Textbox"",""value"":0},{""name"":""Number"",""value"":1},{""name"":""Email"",""value"":2},{""name"":""DatePicker"",""value"":3},{""name"":""RichTextEditor"",""value"":4},{""name"":""MediaEditor"",""value"":5},{""name"":""KeyValueEditor"",""value"":6},{""name"":""LinkEditor"",""value"":7},{""name"":""JsonEditor"",""value"":8}]"
                   },
                },
            }
        }
                },
            };
            versionContext.InsertData(cmsthememanifestsList);

            var cmspagetemplatesList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5fb639108e67fa00011797dd",
                Name = "cmspagetemplates",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Page Templates",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{ \"$query\": { \"pagetemplates\": [ ] } }",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "name",
                DisplayName = "Name",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "themeId",
                DisplayName = "Theme",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Select
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                    Query = "{\"$query\":{\"themes\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
                },

                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "siteId",
                DisplayName = "Site",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Select
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                    Query = "{\"$query\":{\"sites\":[{\"$project\":{\"name\":\"$name\",\"value\":\"$_id\"}}]}}",
                },

                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "create",
                DisplayName = "Create",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.OutList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-page-template"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "edit",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 1,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-page-template?pageTemplateId={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmspagetemplatesList);

            var cmsblogslistList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5fb8a530978a6b0001a024b9",
                Name = "cmsblogslist",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Blogs List",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{ \"$query\": { \"blogs\": [ ] } }",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "id",
                DisplayName = "_id",
                IsHidden = true,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "urlPath",
                DisplayName = "Url Path",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "title",
                DisplayName = "Title",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "creator",
                DisplayName = "Creator",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "modifiedDate",
                DisplayName = "Modified Date",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.DatePicker
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "create",
                DisplayName = "Create",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.OutList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-blog-form"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "edit",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 1,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-blog-form?blogId={{data.id}}"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "posts",
                DisplayName = "Posts",
                Color = "warn",
                Icon = "picture_as_pdf",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 2,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-posts?blogId={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmsblogslistList);

            var cmspostslistList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5fb8e9574f013c00017fd9f6",
                Name = "cmspostslist",
                AppId = "5f23f7d6b8f393672ce21029",
                DisplayName = "CMS Posts List",
                Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>
    {
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "sizeoptions",
            Value = "[ 5, 10, 20, 50 ]",
            Description = "Number of items will be displayed. Default: 5,10,20,50"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "defaultpagesize",
            Value = "10",
            Description = "The default number of items. Default: 10"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "fetchfirsttime",
            Value = "true",
            Description = "Allow calling the data when list is appeared in page. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumcolumns",
            Value = "6",
            Description = "When a number of columns is over this value, Details button will be displayed. Default: 6"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablesearch",
            Value = "true",
            Description = "If it is false, so a search textbox will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableadvancedsearch",
            Value = "true",
            Description = "If it is false, so an advanced search will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enablepagination",
            Value = "true",
            Description = "If it is false, so a pagination will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "enableexportexcel",
            Value = "true",
            Description = "If it is false, so an export button will be disappeared. Default: true"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "maximumclientexport",
            Value = "200",
            Description = "Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100"
        },
        new LetPortal.Portal.Entities.Pages.ShellOption
        {
            Key = "allowexporthiddenfields",
            Value = "false",
            Description = "If it is true, user can export hidden fields. Default: false"
        }
    },
                ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource
                {
                    DatabaseConnectionOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                        Query = "{\"$query\":{\"posts\":[{\"$match\":{\"blogId\":\"{{queryparams.blogId}}\"}}]}}",
                    },

                    SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database
                },
                ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList
                {
                    ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>
        {
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "id",
                DisplayName = "_id",
                IsHidden = true,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "urlPath",
                DisplayName = "Url Path",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "title",
                DisplayName = "Title",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "author",
                DisplayName = "Author",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Text
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "createdDate",
                DisplayName = "Created Date",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = true,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = false,
                    AllowTextSearch = false,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.DatePicker
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
                   {
                        JsonResource = ""
                   },
                },
            },
            new LetPortal.Portal.Entities.SectionParts.ColumnDef
            {
                Name = "blogId",
                DisplayName = "Blog",
                IsHidden = false,
                DisplayFormat = "{0}",
                AllowSort = false,
                HtmlFunction = "",
                DisplayFormatAsHtml = false,
                Order = 0,
                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions
                {
                    AllowInAdvancedMode = true,
                    AllowTextSearch = true,
                    FieldValueType = LetPortal.Portal.Entities.SectionParts.FieldValueType.Select
                },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions
                {
                    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
                    TriggeredEvents = "",
                    OutputMapProjection = "",
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                    Query = "{\"$query\":{\"blogs\":[{\"$project\":{\"name\":\"$title\",\"value\":\"$_id\"}}]}}",
                },

                },
            }
        }
                },
                CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList
                {
                    CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>
        {
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "create",
                DisplayName = "Create",
                Color = "primary",
                Icon = "create",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.OutList,
                Order = 0,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-post-form"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "edit",
                DisplayName = "Edit",
                Color = "primary",
                Icon = "edit",
                AllowRefreshList = false,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 1,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.Redirect,
                    IsEnable = true,
                    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions
                    {
                        IsSameDomain = true,
                        RedirectUrl = "portal/page/cms-post-form?postId={{data.id}}"
                    },
                },

            }
        }
                },
            };
            versionContext.InsertData(cmspostslistList);

            var cmssitesPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f47e21e09012d61101e31bb",
                Name = "cms-sites",
                DisplayName = "CMS Sites",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-sites",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "c676e817-9220-af51-b112-964d9d48a2f6",
                ComponentId = "5f469621c0a5165a80016f71",
                Name = "sites",
                DisplayName = "Sites",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmssitesPage);

            var cmspagesPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f47e27609012d61101e31d8",
                Name = "cms-pages",
                DisplayName = "CMS Pages",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-pages",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "de63cc76-f4ef-609f-8d02-45a67f16b54f",
                ComponentId = "5f47d2e709012d61101e3162",
                Name = "cmspages",
                DisplayName = "CMS Pages",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmspagesPage);

            var cmssitemanifestsPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f47e56d09012d61101e3272",
                Name = "cms-site-manifests",
                DisplayName = "CMS Site Manifests",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-site-manifests",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "7a3a4885-9a2f-3ee1-3027-8fea01ea4ed5",
                ComponentId = "5f49fc17dce086279c4aa461",
                Name = "cmssitemanifest",
                DisplayName = "CMS Site Manifest",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            },
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "35188e2b-1e66-d8c7-52bf-a057ee86c619",
                ComponentId = "5f47e4ed09012d61101e323f",
                Name = "cmssitemanifests",
                DisplayName = "CMS Site Manifests",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "e28289c0-9e1e-c668-fc9e-e0f9d4b3968c",
           Name = "data",
           TriggerCondition = "!!queryparams.id",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"sitemanifests\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.id}}\')\"}}]}" +
    "}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "de882c8d-fc2a-9ade-5807-4d1d0c338c7d",
            Name = "Edit",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "false",
            PlaceSectionId = "7a3a4885-9a2f-3ee1-3027-8fea01ea4ed5",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to update it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"sitemanifests\":{\"$data\":\"{{data}}\",\"$where\":{\"_id\":\"ObjectId(\'{{data" +
    ".id}}\')\"}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmssitemanifestsPage);

            var cmssitePage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f47eeb909012d61101e343e",
                Name = "cms-site",
                DisplayName = "CMS Site",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-site",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "7b177de0-bd1a-a127-9167-9f74d0517c3d",
                ComponentId = "5f47ede309012d61101e3416",
                Name = "cmssite",
                DisplayName = "CMS Site",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "95cc12a1-0864-42f9-1e1f-dc22c8539844",
           Name = "data",
           TriggerCondition = "!!queryparams.id",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"sites\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.id}}\')\"}}]}}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "34359452-55f1-bf1c-231a-2c309dbd0253",
            Name = "Create",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!!queryparams.id",
            PlaceSectionId = "7b177de0-bd1a-a127-9167-9f74d0517c3d",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to save it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$insert\":{\"sites\":{\"$data\":\"{{data}}\"}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "8ba0f62b-d0c0-1b74-ae57-83afd3283162",
            Name = "Update",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!queryparams.id",
            PlaceSectionId = "7b177de0-bd1a-a127-9167-9f74d0517c3d",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to update it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"sites\":{\"$data\":\"{{data}}\",\"$where\":{\"_id\":\"ObjectId(\'{{data.id}}\')\"" +
    "}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "dc723075-58c7-b24e-61c5-8b5618bfcc8f",
            Name = "Cancel",
            Icon = "close",
            Color = "basic",
            AllowHidden = "false",
            PlaceSectionId = "7b177de0-bd1a-a127-9167-9f74d0517c3d",
            IsRequiredValidation = false,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = true,
                    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>
                    {
                        new LetPortal.Portal.Entities.Shared.Route
                        {
                            RedirectUrl = "portal/page/cms-sites",
                            Condition = "true",
                            IsSameDomain = true,
                         },
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmssitePage);

            var cmsthemesPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f4e75b9fc4f674a9c1aa57e",
                Name = "cms-themes",
                DisplayName = "CMS Themes",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-themes",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "2ccd0ad5-c47c-89b6-ddfa-210bd41f066f",
                ComponentId = "5f4e745afc4f674a9c1aa50a",
                Name = "cmsthemes",
                DisplayName = "CMS Themes",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmsthemesPage);

            var cmsthememanifestsPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f4f3148c9957252a4103999",
                Name = "cms-theme-manifests",
                DisplayName = "CMS Theme Manifests",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-theme-manifests",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "e81bfda0-64cd-39aa-525b-97ce75a3c7af",
                ComponentId = "5f4f310ec9957252a4103950",
                Name = "cmsthememanifests",
                DisplayName = "CMS Theme Manifests",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmsthememanifestsPage);

            var cmspagePage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f5276b4775f3a5ee4b2ee14",
                Name = "cms-page",
                DisplayName = "CMS Page",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-page",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "1bb390b8-8cfa-ec20-bfb8-8b232a5312d3",
                ComponentId = "5f527543775f3a5ee4b2ede7",
                Name = "cmspage",
                DisplayName = "CMS Page",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            },
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "c33df3cb-8307-8ef1-014d-4f0caf14d3f4",
                ComponentId = "5f527a1f775f3a5ee4b2ef01",
                Name = "cmspagegooglemetadata",
                DisplayName = "CMS Page Google Metadata",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data.googleMetadata",
                    DataStoreName = "googleMetadata"
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "6ba3a349-9442-a869-ecd6-d21224f92d57",
           Name = "data",
           TriggerCondition = "!!queryparams.siteid && !!queryparams.id",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"pages\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.id}}\')\"}}]}}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "d390891c-5c4d-8f47-4429-604849a69abe",
            Name = "Create",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!!queryparams.id",
            PlaceSectionId = "",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "9b94650b-2243-83a5-6123-2b7f87a739e5",
            Name = "Save",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!queryparams.siteid && !queryparams.id",
            PlaceSectionId = "",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to update it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"pages\":{\"$data\":\"{{data}}\",\"$where\":{\"_id\":\"ObjectId(\'{{data.id}}\')\"" +
    "}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "34fc6936-b983-18d6-bd4d-7b5d4519e0d7",
            Name = "Cancel",
            Icon = "cancel",
            Color = "basic",
            AllowHidden = "false",
            PlaceSectionId = "",
            IsRequiredValidation = false,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = true,
                    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>
                    {
                        new LetPortal.Portal.Entities.Shared.Route
                        {
                            RedirectUrl = "portal/page/cms-pages",
                            Condition = "true",
                            IsSameDomain = true,
                         },
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmspagePage);

            var cmssitemenuPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb60b698e67fa00011791a7",
                Name = "cms-site-menu",
                DisplayName = "CMS Site Menu",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-site-menu",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "b7ce0b78-ea8e-fe1b-dfec-54f9fd2a7634",
                ComponentId = "5fb6b4104f46d90001e67e8b",
                Name = "cmssitemenu",
                DisplayName = "CMS Site Menu",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Tree,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data.menus",
                    DataStoreName = "menus"
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "1f41f2e0-819a-d733-01f9-856ef0929d53",
           Name = "data",
           TriggerCondition = "!!queryparams.siteId",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"menus\":[{\"$match\":{\"siteId\":\"{{queryparams.siteId}}\"}}]}}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "1d0b7c48-b416-3832-a705-96b50888c570",
            Name = "Update",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "false",
            PlaceSectionId = "b7ce0b78-ea8e-fe1b-dfec-54f9fd2a7634",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to update site menu?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"menus\":{\"$data\":{\"menus\":\"{{data.menus}}\"},\"$where\":{\"siteId\":\"{{que" +
    "ryparams.siteId}}\"}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "3d44afab-53be-26a4-8f32-40db244a3522",
            Name = "Cancel",
            Icon = "cancel",
            Color = "basic",
            AllowHidden = "false",
            PlaceSectionId = "b7ce0b78-ea8e-fe1b-dfec-54f9fd2a7634",
            IsRequiredValidation = false,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = true,
                    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>
                    {
                        new LetPortal.Portal.Entities.Shared.Route
                        {
                            RedirectUrl = "portal/page/cms-sites",
                            Condition = "true",
                            IsSameDomain = true,
                         },
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmssitemenuPage);

            var cmspagetemplatesPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb6392a8e67fa00011797e0",
                Name = "cms-page-templates",
                DisplayName = "CMS Page Templates",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-page-templates",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "48c033e4-7b51-5cf7-2b2a-bf7912b62d10",
                ComponentId = "5fb639108e67fa00011797dd",
                Name = "cmspagetemplates",
                DisplayName = "CMS Page Templates",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmspagetemplatesPage);

            var cmspagetemplatePage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb69663ea2c730001a775f6",
                Name = "cms-page-template",
                DisplayName = "CMS Page Template",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-page-template",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "3d59276a-8084-61ea-cc91-94fd3513ff5a",
                ComponentId = "5fb69456ea2c730001a775ad",
                Name = "cmspagetemplateinfo",
                DisplayName = "CMS Page Template Info",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            },
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "376b6e11-1f00-6772-3522-54d84579800c",
                ComponentId = "5fb761050383a10001ba17c2",
                Name = "cmspagetemplatesections",
                DisplayName = "CMS Page Template Sections",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Array,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data.sections",
                    DataStoreName = "sections"
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "7957cc25-12b4-c6bb-ae0f-e81236ec9ccc",
           Name = "data",
           TriggerCondition = "!!queryparams.pageTemplateId",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"pagetemplates\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.pageTemplat" +
    "eId}}\')\"}}]}}",
            },

        },

        },
    },
            };
            versionContext.InsertData(cmspagetemplatePage);

            var cmsblogsPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb8a61f978a6b0001a024da",
                Name = "cms-blogs",
                DisplayName = "CMS Blogs",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-blogs",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "56bbd73f-0c2d-ac39-d438-118adbdc541f",
                ComponentId = "5fb8a530978a6b0001a024b9",
                Name = "cmsblogslist",
                DisplayName = "CMS Blogs List",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmsblogsPage);

            var cmsblogformPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb8aad7978a6b0001a0258b",
                Name = "cms-blog-form",
                DisplayName = "CMS Blog Form",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-blog-form",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "f222120d-123a-abce-878c-1806b3122732",
                ComponentId = "5fb8a918978a6b0001a0254c",
                Name = "cmsblogform",
                DisplayName = "CMS Blog Form",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "349741ea-e6b4-ab5c-3865-c4b0b0813600",
           Name = "data",
           TriggerCondition = "!!queryparams.blogId",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"blogs\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.blogId}}\')\"}}]}}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "9b1862de-b208-4578-b89d-d837a22c20e4",
            Name = "Create",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!!queryparams.blogId",
            PlaceSectionId = "f222120d-123a-abce-878c-1806b3122732",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$insert\":{\"blogs\":{\"$data\":\"{{data}}\",\"creator\":\"{{user.username}}\",\"createdDat" +
    "e\":\"ISODate(\'{{currentISODate()}}\')\",\"updatedDate\":\"ISODate(\'{{currentISODate()}" +
    "}\')\"}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "8a2e64bb-3032-a930-6cf8-3321d4a9c85e",
            Name = "Update",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!queryparams.blogId",
            PlaceSectionId = "f222120d-123a-abce-878c-1806b3122732",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"blogs\":{\"$data\":\"{{data}}\",\"modifiedDate\":\"ISODate(\'{{currentISODate" +
    "()}}\')\",\"$where\":{\"_id\":\"ObjectId(\'{{data.id}}\')\"}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "ea986631-c87f-f4d8-5aa1-23964e58894c",
            Name = "Cancel",
            Icon = "cancel",
            Color = "basic",
            AllowHidden = "false",
            PlaceSectionId = "f222120d-123a-abce-878c-1806b3122732",
            IsRequiredValidation = false,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = true,
                    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>
                    {
                        new LetPortal.Portal.Entities.Shared.Route
                        {
                            RedirectUrl = "portal/page/cms-blogs",
                            Condition = "true",
                            IsSameDomain = true,
                         },
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmsblogformPage);

            var cmspostsPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb8e9ed4f013c00017fda0d",
                Name = "cms-posts",
                DisplayName = "CMS Posts",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-posts",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "a2a5ff92-3aaf-e0f0-c768-e2cd28a08cfd",
                ComponentId = "5fb8e9574f013c00017fd9f6",
                Name = "cmspostslist",
                DisplayName = "CMS Posts List",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.DynamicList,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

            };
            versionContext.InsertData(cmspostsPage);

            var cmspostformPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fb8f0114f013c00017fdae4",
                Name = "cms-post-form",
                DisplayName = "CMS Post Form",
                AppId = "5f23f7d6b8f393672ce21029",
                UrlPath = "portal/page/cms-post-form",
                Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>
    {
        new LetPortal.Core.Security.PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Page Access",
            ClaimValueType = LetPortal.Core.Security.ClaimValueType.Boolean
        },
    },
                Builder = new LetPortal.Portal.Entities.Pages.PageBuilder
                {
                    Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>
        {
            new LetPortal.Portal.Entities.Pages.PageSection
            {
                Id = "687424e5-dca4-26db-3656-207ce459dd8a",
                ComponentId = "5fb8ee954f013c00017fdaae",
                Name = "cmspostform",
                DisplayName = "CMS Post Form",
                ConstructionType = LetPortal.Portal.Entities.Pages.SectionContructionType.Standard,
                Hidden = "false",
                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource
                {
                    DatasourceBindName = "data",
                    DataStoreName = ""
                },
            }
        }
                },

                PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>
    {
        new LetPortal.Portal.Entities.Pages.PageDatasource
        {
           Id = "9d92a1c8-cf7b-043f-467e-f38c79d5f015",
           Name = "data",
           TriggerCondition = "!!queryparams.postId",
           IsActive = true,
        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
        {
            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
            {
                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                Query = "{\"$query\":{\"posts\":[{\"$match\":{\"_id\":\"ObjectId(\'{{queryparams.postId}}\')\"}}]}}",
            },

        },

        },
    },
                Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>
    {
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "36bc101d-00ce-0baf-8f37-186949ff51ef",
            Name = "Create",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!!queryparams.postId",
            PlaceSectionId = "687424e5-dca4-26db-3656-207ce459dd8a",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$insert\":{\"posts\":{\"$data\":\"{{data}}\",\"author\":\"{{user.username}}\",\"createdDate" +
    "\":\"ISODate(\'{{currentISODate()}}\')\"}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "eee2fd2c-578b-b131-8f02-9ae1a914fb5c",
            Name = "Update",
            Icon = "edit",
            Color = "primary",
            AllowHidden = "!queryparams.postId",
            PlaceSectionId = "687424e5-dca4-26db-3656-207ce459dd8a",
            IsRequiredValidation = true,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "Completed!",
                        FailedMessage = "Oops! Something went wrong, please try again!",
                    },
                    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains
                    {
                        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>
                        {
                            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep
                            {
                                DatabaseConnectionId = "5f33c1ddedf7b3de91e106d8",
                                ExecuteCommand = "{\"$update\":{\"posts\":{\"$data\":\"{{data}}\",\"$where\":{\"_id\":\"ObjectId(\'{{data.id}}\')\"" +
    "}}}}",
                                DataLoopKey = "",
                            }
                       }
                    },
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = false,
                },

            },

        },
        new LetPortal.Portal.Entities.Pages.PageButton
        {
            Id = "2553b36e-cdfc-33a4-a8ac-94f805571a1e",
            Name = "Cancel",
            Icon = "cancel",
            Color = "basic",
            AllowHidden = "false",
            PlaceSectionId = "687424e5-dca4-26db-3656-207ce459dd8a",
            IsRequiredValidation = false,
            ButtonOptions = new LetPortal.Portal.Entities.Shared.ButtonOptions
            {
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.ExecuteDatabase,
                    IsEnable = false,
                },

                RouteOptions = new LetPortal.Portal.Entities.Shared.RouteOptions
                {
                    IsEnable = true,
                    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>
                    {
                        new LetPortal.Portal.Entities.Shared.Route
                        {
                            RedirectUrl = "portal/page/cms-blogs",
                            Condition = "true",
                            IsSameDomain = true,
                         },
                    },
                },

            },

        },
    },
            };
            versionContext.InsertData(cmspostformPage);

            var cmsmediaStandard = new LetPortal.Portal.Entities.Components.Controls.CompositeControl
            {
                Id = "5fcbb5aee19fcf0001a2d90d",
                Name = "cmsmedia",
                DisplayName = "CMS Media",
                AppId = "5f23f7d6b8f393672ce21029",
                Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>
    {
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "mediaType",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 3,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Media Type",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "mediaType",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "0",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
    DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
    {
        JsonResource = "[{\"name\":\"Image\",\"value\":0},{\"name\":\"Video\",\"value\":1}]"
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please select one option",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "link",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Uploader,
            Order = 0,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Link",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "link",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "allowfileurl",
                    Value = "false",
                    Description = "Allow an uploader set downloadable url back to a field after saving instead of file id. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "saveonchange",
                    Value = "false",
                    Description = "Allow an uploader upload a file after user changes. Default: false",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please upload one file",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.FileMaximumSize,
                    IsActive = true,
                    ValidatorMessage = "File must have size less than {{option}} Mb",
                    ValidatorOption = "10",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.FileExtensions,
                    IsActive = true,
                    ValidatorMessage = "File must have an extension in {{option}}",
                    ValidatorOption = "jpg;jpeg;png;gif",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "targetLink",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Select,
            Order = 1,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Target Link",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "targetLink",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "multiple",
                    Value = "false",
                    Description = "Multiple options can be selected. Default: false",
                },
            },
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,
    DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions
    {
        JsonResource = "[{\"name\":\"Blank\",\"value\":\"_blank\"},{\"name\":\"Parent\",\"value\":\"_parent\"},{\"name\":\"S" +
    "elf\",\"value\":\"_self\"},{\"name\":\"Top\",\"value\":\"_top\"}]"
    },

},

            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please select one option",
                    ValidatorOption = "",
                },
            },
            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>
            {
            new LetPortal.Portal.Entities.Components.Controls.PageControlEvent
{
    EventName = "_change",
    EventActionType = LetPortal.Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
EventHttpServiceOptions = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions
{
    HttpServiceUrl = "",
    HttpMethod = "Get",
    HttpSuccessCode = "200",
    JsonBody = "",
    OutputProjection = "",
},

EventDatabaseOptions = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions
{
    DatabaseConnectionId = "",
    Query = "",
    OutputProjection = "",
},

TriggerEventOptions = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions
{
}

}

            },
        },
        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl
        {
            Name = "alt",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 2,
            CompositeControlId = "",
            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>
            {
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "label",
                    Value = "Alt",
                    Description = "Label will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "placeholder",
                    Value = "Enter alt text",
                    Description = "Placeholder will be displayed when it isn't empty",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "disabled",
                    Value = "false",
                    Description = "Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "hidden",
                    Value = "false",
                    Description = "Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "bindname",
                    Value = "alt",
                    Description = "Bind Name is a name which helps to map the data in or out",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "defaultvalue",
                    Value = "",
                    Description = "Default value when no value is set",
                },
                new LetPortal.Portal.Entities.Pages.ShellOption
                {
                    Key = "rendered",
                    Value = "true",
                    Description = "Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true",
                },
            },
            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>
            {
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                    IsActive = true,
                    ValidatorMessage = "Please fill out this field",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires at least {{option}} characters",
                    ValidatorOption = "5",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                    IsActive = true,
                    ValidatorMessage = "This field requires maximum {{option}} characters",
                    ValidatorOption = "250",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                    IsActive = false,
                    ValidatorMessage = "This field's format does not match",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                    IsActive = false,
                    ValidatorMessage = "Please input correct email",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                    IsActive = false,
                    ValidatorMessage = "This field must be equal {{option}}",
                    ValidatorOption = "",
                },
                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator
                {
                    ValidatorType = LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                    IsActive = false,
                    ValidatorMessage = "This field doesn't match with {{option}} field",
                    ValidatorOption = "",
                },
            },
        }
    },
            };
            versionContext.InsertData(cmsmediaStandard);

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
