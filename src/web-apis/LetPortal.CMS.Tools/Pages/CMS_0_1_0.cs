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
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.StandardComponent>("5f5890345e6a3b5f54192051");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f469621c0a5165a80016f71");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f47d2e709012d61101e3162");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f47e4ed09012d61101e323f");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f4e745afc4f674a9c1aa50a");
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5f4f310ec9957252a4103950");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e21e09012d61101e31bb");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e27609012d61101e31d8");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47e56d09012d61101e3272");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f47eeb909012d61101e343e");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f4e75b9fc4f674a9c1aa57e");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f4f3148c9957252a4103999");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5f5276b4775f3a5ee4b2ee14");
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
                Id = "5e1aa91e3c107562acf358b3",
                DisplayName = "Themes",
                Icon = "backup_table",
                Url = "/portal/page/cms-themes",
                MenuPath = "~/5f258a65807fec5a88e22c2e",
                Order = 1,
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
                ConnectionString = "mongodb://localhost:27017/cms",
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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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

            var cmspagetemplateStandard = new LetPortal.Portal.Entities.SectionParts.StandardComponent
            {
                Id = "5f5890345e6a3b5f54192051",
                Name = "cmspagetemplate",
                DisplayName = "CMS Page Template",
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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
            Name = "name",
            Type = LetPortal.Portal.Entities.SectionParts.Controls.ControlType.Textbox,
            Order = 1,
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
                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DatasourceOptions
{
    Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource,

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
        }
    },
            };
            versionContext.InsertData(cmspagetemplateStandard);

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

            var cmssitesPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5f47e21e09012d61101e31bb",
                Name = "cms-sites",
                DisplayName = "CMS Sites",
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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
                AppId = "5c162e9005924c1c741bfdc2",
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

            },

        },
    },
            };
            versionContext.InsertData(cmspagePage);

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
