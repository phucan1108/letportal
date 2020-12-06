using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Pages
{
    public class Page_0_3_0 : IPortalVersion
    {
        public string VersionNumber => "0.3.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5d64e0dc6a1a49378cef5c70");
            versionContext.DeleteData<Page>("5d63423dbaac7d4790b7d301");
            versionContext.DeleteData<Page>("5d63423dbaac7d4790b7d302");
            versionContext.DeleteData<Page>("5dabf30467cb8d0bd02643f9");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var rolePage = new Page
            {
                Id = "5d63423dbaac7d4790b7d301",
                Name = "role-form",
                DisplayName = "Role Form",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/role-form",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                ShellOptions = new List<ShellOption>
                    {
                        new ShellOption
                        {
                            Key = "entityname",
                            Value = "roles"
                        }
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                           Id = "5d63423dbaac7d4790b7d306",
                           ComponentId ="5d63423dbaac7d4790b7d300",
                           ConstructionType = SectionContructionType.Standard,
                           Name = "roleForm",
                           DisplayName = "Role Form",
                           SectionDatasource = new SectionDatasource
                           {
                               DatasourceBindName = "data"
                           }
                        }
                    }
                },
                PageDatasources = new List<PageDatasource>
                {
                    new PageDatasource
                    {
                        Id = "e796a706-3281-9815-bb9a-0df36bed535f",
                        Name = "data",
                        IsActive = true,
                        TriggerCondition = "!!queryparams.id",
                        Options = new Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = Portal.Entities.Shared.DatasourceControlType.Database,
                            DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                            {
                                DatabaseConnectionId = Constants.IdentityDatabaseId,
                                EntityName = "roles",
                                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                    "{\"$query\":{\"roles\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `roles` Where id={{queryparams.id}}" : "Select * from roles Where id={{queryparams.id}}")
                            }
                        }
                    }
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "180b55dd-e31f-387f-0844-7a9c74f88941",
                        Name = "Save",
                        Icon = "save",
                        Color = "primary",
                        AllowHidden = "!!queryparams.id",
                        PlaceSectionId = "5d63423dbaac7d4790b7d306",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {                               
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DbExecutionChains = new DatabaseExecutionChains
                                {
                                    Steps = new List<DatabaseExecutionStep>
                                    {
                                        new DatabaseExecutionStep
                                        {
                                            DatabaseConnectionId = Constants.IdentityDatabaseId,
                                            ExecuteCommand = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                                "{\r\n  \"$insert\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"normalizedName\": \"{{data.name.toUpperCase()}}\" }\r\n  }\r\n}"
                                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                                                        ? "INSERT INTO `roles` (id, name, `displayName`, `normalizedName`) Values ({{guid()}}, {{data.name}}, {{data.displayName}}, {{data.name.toUpperCase()}})"
                                                        : "INSERT INTO roles(id, name, \"displayName\", \"normalizedName\") Values ({{guid()}}, {{data.name}}, {{data.displayName}}, {{data.name.toUpperCase()}})")
                                        }
                                    }
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Insert new role successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to create a role?"
                                }
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {
                                        Condition = "true",                                        
                                        RedirectUrl = "portal/page/roles-management",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }

                    },
                    new PageButton
                    {
                        Id = "5253c309-e250-dc62-c322-c54d510f78c9",
                        Name = "Update",
                        Icon = "edit",
                        Color = "primary",
                        AllowHidden = "!queryparams.id",
                        PlaceSectionId = "5d63423dbaac7d4790b7d306",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {                               
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DbExecutionChains = new DatabaseExecutionChains
                                {
                                    Steps = new List<DatabaseExecutionStep>
                                    {
                                        new DatabaseExecutionStep
                                        {
                                            DatabaseConnectionId = Constants.IdentityDatabaseId,
                                            ExecuteCommand = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                                "{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",  \"$where\": {\r\n        \"_id\": \"ObjectId('{{data.id}}')\"\r\n      }\r\n    }\r\n  }\r\n}"
                                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                                                        ? "UPDATE `roles` SET name={{data.name}}, `displayName`={{data.displayName}}, `normalizedName`={{data.name.toUpperCase()}} Where id={{data.id}}"
                                                        : "UPDATE roles SET name={{data.name}}, \"displayName\"={{data.displayName}}, \"normalizedName\"={{data.name.toUpperCase()}} Where id={{data.id}}")
                                        }
                                    }
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update role successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to update a role?"
                                }
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = false
                            }
                        }
                    },
                    new PageButton
                    {
                        Id = "5eaebdb89533066fa085a844",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        PlaceSectionId = "5d63423dbaac7d4790b7d306",
                        ButtonOptions = new ButtonOptions
                        {                               
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = false
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {                                           
                                        Condition = "true",
                                        RedirectUrl = "portal/page/roles-management",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var registerUserPage = new Page
            {
                Id = "5d63423dbaac7d4790b7d302",
                Name = "add-user-form",
                DisplayName = "Add User",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/add-user-form",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                            Id = "5eaebdb89533066fa085a845",
                            ComponentId = "5d6222ec3aae6a79ecf035e9",
                            ConstructionType = SectionContructionType.Standard,
                            DisplayName = "Add User",
                            Name = "addUser",
                            SectionDatasource = new SectionDatasource
                            {
                                DatasourceBindName = "data"
                            }
                        }
                    }
                },
                PageDatasources = new List<PageDatasource>(),
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "5eaebdb89533066fa085a846",
                        Name = "Save",
                        Color = "primary",
                        Icon = "save",
                        PlaceSectionId = "5eaebdb89533066fa085a845",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {                               
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.CallHttpService,
                                IsEnable = true,
                                HttpServiceOptions = new HttpServiceOptions
                                {
                                    HttpMethod = "Post",
                                    HttpServiceUrl = "{{configs.identityBaseEndpoint}}/api/accounts/register",
                                    JsonBody = "{\r\n  \"username\": \"{{data.username}}\",\r\n  \"password\": \"{{data.password}}\",\r\n  \"repassword\": \"{{data.repassword}}\",\r\n  \"email\": \"{{data.email}}\"\r\n}",
                                    HttpSuccessCode = "200;204",
                                    OutputProjection = ""
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Add user successfully!",
                                    FailedMessage = "Oops, something went wrong, please check again"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to create a new user?"
                                }
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {
                                        Condition = "true",                                        
                                        RedirectUrl = "portal/page/users-management",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }
                    },
                    new PageButton
                    {
                        Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a852",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        PlaceSectionId = "5eaebdb89533066fa085a845",
                        ButtonOptions = new ButtonOptions
                        {                                
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = false
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {
                                        RedirectUrl = "portal/page/users-management",
                                        Condition = "true",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var userFormPage = new Page
            {
                Id = "5d64e0dc6a1a49378cef5c70",
                Name = "user-form",
                DisplayName = "User Form",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/user-form",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                ShellOptions = new List<ShellOption>
                    {
                        new ShellOption
                        {
                            Key = "entityname",
                            Value = "users"
                        }
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                           Id = "5d63423dbaac7d4790b7d306",
                           ComponentId ="5d615e501773a96ee8eb5ed3",
                           ConstructionType = SectionContructionType.Standard,
                           Name = "userform",
                           DisplayName = "User Form",
                           SectionDatasource = new SectionDatasource
                           {
                               DatasourceBindName = "data"
                           }
                        }
                    }
                },
                PageDatasources = new List<PageDatasource>
                {
                    new PageDatasource
                    {
                        Id = "e796a706-3281-9815-bb9a-0df36bed535f",
                        Name = "data",
                        IsActive = true,
                        TriggerCondition = "!!queryparams.id",
                        Options = new Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = Portal.Entities.Shared.DatasourceControlType.Database,
                            DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                            {
                                DatabaseConnectionId = Constants.IdentityDatabaseId,
                                EntityName = "roles",
                                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                    "{\"$query\":{\"users\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `users` Where id={{queryparams.id}}" : "Select * from users Where id={{queryparams.id}}")
                            }
                        }
                    }
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "180b55dd-e31f-387f-0844-7a9c74f88941",
                        Name = "Update",
                        Icon = "edit",
                        Color = "primary",
                        AllowHidden = "!queryparams.id",
                        PlaceSectionId = "5d63423dbaac7d4790b7d306",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {                                
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DbExecutionChains = new DatabaseExecutionChains
                                {
                                    Steps = new List<DatabaseExecutionStep>
                                    {
                                        new DatabaseExecutionStep
                                        {
                                            DatabaseConnectionId = Constants.IdentityDatabaseId,
                                            ExecuteCommand = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                                "{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\", \"normalizedEmail\":\"{{data.email.toUpperCase()}}\",  \"$where\": {\r\n        \"_id\": \"ObjectId('{{data.id}}')\"\r\n      }\r\n    }\r\n  }\r\n}"
                                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                                                        ? "Update `users` Set email={{data.email}}, `normalizedEmail`={{data.email.toUpperCase()}}, `isConfirmedEmail`={{data.isConfirmedEmail|bool}}, `isLockoutEnabled`={{data.isLockoutEnabled|bool}}, `lockoutEndDate`={{data.lockoutEndDate|date}}, roles={{toJsonString(data.roles)}} Where id={{data.id}}"
                                                        : "Update users Set email={{data.email}}, \"normalizedEmail\"={{data.email.toUpperCase()}}, \"isConfirmedEmail\"={{data.isConfirmedEmail|bool}}, \"isLockoutEnabled\"={{data.isLockoutEnabled|bool}}, \"lockoutEndDate\"={{data.lockoutEndDate|date}}, roles={{toJsonString(data.roles)}} Where id={{data.id}}")
                                        }
                                    }
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update user successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to update a user?"
                                }
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {
                                        Condition = "true",
                                        RedirectUrl = "portal/page/users-management",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }

                    },
                    new PageButton
                    {
                        Id = "5eaebdb89533066fa085a847",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        PlaceSectionId = "5d63423dbaac7d4790b7d306",
                        ButtonOptions = new ButtonOptions
                        {                                
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = false
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {                                           
                                        Condition = "true",
                                        RedirectUrl = "portal/page/users-management",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var chartListsPage = new Page
            {
                Id = "5dabf30467cb8d0bd02643f9",
                Name = "charts-management",
                DisplayName = "Charts Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/charts-management",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                            Id = "5d0c7da473e71f3330054792",
                            Name = "chartslist",
                            DisplayName = "Charts List",
                            ComponentId = "5dabf30467cb8d0bd02643f8",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            versionContext.InsertData(userFormPage);
            versionContext.InsertData(rolePage);
            versionContext.InsertData(registerUserPage);
            versionContext.InsertData(chartListsPage);

            return Task.CompletedTask;
        }
    }
}
