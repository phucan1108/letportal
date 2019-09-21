using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Versions.Pages
{
    public class Page_0_0_3 : IVersion
    {
        public string VersionNumber => "0.0.3";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5d64e0dc6a1a49378cef5c70");
            versionContext.DeleteData<Page>("5d63423dbaac7d4790b7d301");
            versionContext.DeleteData<Page>("5d63423dbaac7d4790b7d302");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var rolePage = new Page
            {
                Id = "5d63423dbaac7d4790b7d301",
                Name = "role-form",
                DisplayName = "Role Form",
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
                           Name = "roleform",
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
                            DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                            {
                                DatabaseConnectionId = Constants.CoreDatabaseId,
                                EntityName = "roles",
                                Query = "{\"$query\":{\"roles\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
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
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = true,
                                ConfirmationText = "Are you sure to create a role?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = "{\r\n  \"$insert\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"normalizedName\": \"{{data.name.toUpperCase()}}\" }\r\n  }\r\n}"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Insert new role successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/roles-management"
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
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                               IsEnable = true,
                               ConfirmationText = "Are you sure to update a role?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = "{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",  \"$where\": {\r\n        \"_id\": \"ObjectId('{{data.id}}')\"\r\n      }\r\n    }\r\n  }\r\n}"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update role successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
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
                        Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a854",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = false
                            },
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/roles-management",
                                        Condition = "true"
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
                            Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a854",
                            ComponentId = "5d6222ec3aae6a79ecf035e9",
                            ConstructionType = SectionContructionType.Standard,
                            DisplayName = "Add User",
                            Name = "adduser",
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
                        Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a854",
                        Name = "Save",
                        Color = "primary",
                        Icon = "save",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = true,
                                ConfirmationText = "Are you sure to create a new user?"
                            },
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/users-management"
                                    }
                                }
                            }
                        }
                    },
                    new PageButton
                    {
                        Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a854",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = false
                            },
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/users-management",
                                        Condition = "true"
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
                            DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                            {
                                DatabaseConnectionId = Constants.CoreDatabaseId,
                                EntityName = "roles",
                                Query = "{\"$query\":{\"users\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
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
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = true,
                                ConfirmationText = "Are you sure to update a user?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = "{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\", \"normalizedEmail\":\"{{data.email.toUpperCase()}}\",  \"$where\": {\r\n        \"_id\": \"ObjectId('{{data.id}}')\"\r\n      }\r\n    }\r\n  }\r\n}"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update user successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/users-management"
                                    }
                                }
                            }
                        }

                    },
                    new PageButton
                    {
                        Id = "48a0b57a-0944-945e-e2f6-c9bcadf9a854",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        ButtonOptions = new ButtonOptions
                        {
                            ConfirmationOptions = new ConfirmationOptions
                            {
                                IsEnable = false
                            },
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
                                        RouteType = RouteType.ThroughUrl,
                                        TargetUrl = "portal/page/users-management",
                                        Condition = "true"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(userFormPage);
            versionContext.InsertData(rolePage);
            versionContext.InsertData(registerUserPage);
        }
    }
}
