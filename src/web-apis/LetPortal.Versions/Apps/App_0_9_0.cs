using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;

namespace LetPortal.Versions.Apps
{
    public class App_0_9_0 : IPortalVersion
    {
        public string VersionNumber => "0.9.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>("5fc7a504648b760001995130");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fc7a521648b760001995133");
            versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>("5fcc97e8a63a2b000198620b");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {

            versionContext.DeleteData<LetPortal.Portal.Entities.Apps.App>("5c162e9005924c1c741bfdc2");
            var coreappApp = new LetPortal.Portal.Entities.Apps.App
            {
                Id = "5c162e9005924c1c741bfdc2",
                Name = "core-app",
                Logo = "important_devices",
                Author = "Admin",
                DefaultUrl = "/portal/page/pages-management",
                CurrentVersionNumber = "0.0.1",
                DisplayName = "Core",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Menus = new List<LetPortal.Portal.Entities.Menus.Menu>
    {
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5cf616bc462b56ee3bc2c7e1",
            DisplayName = "Core",
            Icon = "settings",
            Url = "#",
            MenuPath = "~",
            Order = 0,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                DisplayName = "Apps Management",
                Icon = "camera_enhance",
                Url = "/portal/page/apps-management",
                MenuPath = "~/5cf616bc462b56ee3bc2c7e1",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                DisplayName = "Databases Management",
                Icon = "view_module",
                Url = "/portal/page/databases-management",
                MenuPath = "~/5cf616bc462b56ee3bc2c7e1",
                Order = 1,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5e1aa91e3c107562acf358b3",
                DisplayName = "Backup Management",
                Icon = "backup",
                Url = "/portal/page/backup-management",
                MenuPath = "~/5cf616bc462b56ee3bc2c7e1",
                Order = 1,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "",
                DisplayName = "Composite Controls",
                Icon = "perm_data_setting",
                Url = "portal/page/composite-controls",
                MenuPath = "",
                Order = 0,
             },
            }
        },
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5d4d8adfae5f5b68b811ec24",
            DisplayName = "Page Settings",
            Icon = "pages",
            Url = "#",
            MenuPath = "~",
            Order = 0,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "cc0efc5d-f80a-f33e-35e2-5d0c12b818c5",
                DisplayName = "Standards Management",
                Icon = "category",
                Url = "/portal/page/standard-list-management",
                MenuPath = "~/5d4d8adfae5f5b68b811ec24",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24ea",
                DisplayName = "Dynamic List Management",
                Icon = "view_module",
                Url = "/portal/page/dynamic-list-management",
                MenuPath = "~/5d4d8adfae5f5b68b811ec24",
                Order = 1,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "172fd9c3-0168-90d1-5de7-60b0acaa24c2",
                DisplayName = "Charts Management",
                Icon = "bar_chart",
                Url = "/portal/page/charts-management",
                MenuPath = "~/5d4d8adfae5f5b68b811ec24",
                Order = 2,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5cf618cd9ec1d3bf5c339614",
                DisplayName = "Pages Management",
                Icon = "receipt",
                Url = "/portal/page/pages-management",
                MenuPath = "~/5d4d8adfae5f5b68b811ec24",
                Order = 3,
             },
            }
        },
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5cf61767db0f4125341873c1",
            DisplayName = "Identity",
            Icon = "verified_user",
            Url = "#",
            MenuPath = "~",
            Order = 1,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5cf61827f2a8d9aa14fab275",
                DisplayName = "Users Management",
                Icon = "assignment_ind",
                Url = "/portal/page/users-management",
                MenuPath = "~/5cf61767db0f4125341873c1",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5cf6188388edcca91ef04879",
                DisplayName = "Roles Management",
                Icon = "card_membership",
                Url = "/portal/page/roles-management",
                MenuPath = "~/5cf61767db0f4125341873c1",
                Order = 1,
             },
            }
        },
        new LetPortal.Portal.Entities.Menus.Menu
        {
            Id = "5dcac739be0b4e533408344b",
            DisplayName = "Services",
            Icon = "memory",
            Url = "#",
            MenuPath = "~",
            Order = 1,
            SubMenus = new List<LetPortal.Portal.Entities.Menus.Menu>
            {
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5cf61827f2a8d9aa14fab275",
                DisplayName = "Services Monitor",
                Icon = "system_update_alt",
                Url = "/portal/page/services-monitor",
                MenuPath = "~/5dcac739be0b4e533408344b",
                Order = 0,
             },
            new LetPortal.Portal.Entities.Menus.Menu
            {
                Id = "5cf6188388edcca91ef04879",
                DisplayName = "Service Logs",
                Icon = "event_note",
                Url = "/portal/page/service-logs",
                MenuPath = "~/5dcac739be0b4e533408344b",
                Order = 1,
             },
            }
        },
    },
            };
            versionContext.InsertData(coreappApp);

            var compositecontrolsList = new LetPortal.Portal.Entities.SectionParts.DynamicList
            {
                Id = "5fc7a504648b760001995130",
                Name = "compositecontrols",
                AppId = "5c162e9005924c1c741bfdc2",
                DisplayName = "Composite Controls",
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
                        DatabaseConnectionId = "5c17b4df7cf5e34530d103b9",
                        Query = "{ \"$query\": { \"compositecontrols\": [ ] } }",
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
                Name = "displayName",
                DisplayName = "Display Name",
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
                Name = "appId",
                DisplayName = "App",
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
                    DatabaseConnectionId = "5c17b4df7cf5e34530d103b9",
                    Query = "{\"$query\":{\"apps\":[{\"$project\":{\"name\":\"$displayName\",\"value\":\"$_id\"}}]}}",
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
                        RedirectUrl = "portal/builder/composite-control-builder"
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
                        RedirectUrl = "portal/builder/composite-control-builder/{{data.id}}"
                    },
                },

            },
            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList
            {
                Name = "delete",
                DisplayName = "Delete",
                Color = "warn",
                Icon = "delete",
                AllowRefreshList = true,
                CommandPositionType = LetPortal.Portal.Entities.SectionParts.CommandPositionType.InList,
                Order = 2,
                ActionCommandOptions = new LetPortal.Portal.Entities.Shared.ActionCommandOptions
                {
                    ActionType = LetPortal.Portal.Entities.Shared.ActionType.CallHttpService,
                    IsEnable = true,
                    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions
                    {
                        IsEnable = true,
                        ConfirmationText = "Are you sure to proceed it?",
                    },
                    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions
                    {
                        CompleteMessage = "The control has been deleted successfully!",
                        FailedMessage = "Oops! We can't delete the control",
                    },
                    HttpServiceOptions = new LetPortal.Portal.Entities.Shared.HttpServiceOptions
                    {
                        HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/composite-controls/{{data.id}}",
                        HttpMethod = "Delete",
                        HttpSuccessCode = "200",
                        JsonBody = "",
                        OutputProjection = "",
                    },

                },

            }
        }
                },
            };
            versionContext.InsertData(compositecontrolsList);

            var compositecontrolsPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fc7a521648b760001995133",
                Name = "composite-controls",
                DisplayName = "Composite Controls",
                AppId = "5c162e9005924c1c741bfdc2",
                UrlPath = "portal/page/composite-controls",
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
                Id = "24bfc6d7-4b7a-61e5-0495-da18ed8f95ca",
                ComponentId = "5fc7a504648b760001995130",
                Name = "compositecontrols",
                DisplayName = "Composite Controls",
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
            versionContext.InsertData(compositecontrolsPage);

            var compositecontrolbuilderPage = new LetPortal.Portal.Entities.Pages.Page
            {
                Id = "5fcc97e8a63a2b000198620b",
                Name = "composite-control-builder",
                DisplayName = "Composite Control Builder",
                AppId = "5c162e9005924c1c741bfdc2",
                UrlPath = "portal/build/composite-control-builder",
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
                },

            };
            versionContext.InsertData(compositecontrolbuilderPage);

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
