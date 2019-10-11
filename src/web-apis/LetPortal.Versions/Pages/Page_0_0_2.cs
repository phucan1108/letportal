using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Versions.Pages
{
    public class Page_0_0_2 : IVersion
    {
        public string VersionNumber => "0.0.2";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5d2607a9e88d28422061cfd0");
            versionContext.DeleteData<Page>("5d35ce3515066059bc5dfd01");
            versionContext.DeleteData<Page>("5d4d8adfae5f5b68b811ec20");
            versionContext.DeleteData<Page>("5d4d8adfae5f5b68b811ec21");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseFormPage = new Page
            {
                Id = "5d2607a9e88d28422061cfd0",
                Name = "database-form",
                DisplayName = "Database Form",
                UrlPath = "portal/page/database-form",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                ShellOptions = new List<ShellOption>
                    {
                        new ShellOption
                        {
                            Key = "entityname",
                            Value = "databases"
                        }
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                            Id = "d69b69db-9850-15aa-08cf-ee6b3c1afa81",
                            Name = "databaseinfo",
                            DisplayName = "Database Info",
                            ConstructionType = SectionContructionType.Standard,
                            ComponentId = "5d25f6abe88d28422061cfaf",
                            Order = 0,
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
                                EntityName = "databases",
                                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                "{\"$query\":{\"databases\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
                                : "Select * From databases Where id={{queryparams.id}}"
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
                                ConfirmationText = "Are you sure to create a database?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                    "{\"$insert\":{\"{{options.entityname}}\":{ \"$data\": \"{{data}}\"}}}"
                                    : "INSERT INTO databases(id, name, displayName, timeSpan, connectionString, dataSource, databaseConnectionType) Values ({{guid()}}, {{data.name}}, {{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.datasource}}, {{data.databaseConnectionType}})"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Insert new database successfully!",
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
                                ConfirmationText = "Are you sure to update a database?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                    "{\"$update\":{\"{{options.entityname}}\":{\"$data\":\"{{data}}\",\"$where\":{\"_id\":\"ObjectId('{{data.id}}')\"}}}}"
                                    : "Update databases SET name={{data.name}}, displayName={{data.displayName}}, timeSpan={{currentTick()}}, connectionString={{data.connectionString}}, dataSource={{data.datasource}}, databaseConnectionType={{data.databaseConnectionType}}"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update database successfully!",
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
                                            Condition = "true",
                                            RouteType = RouteType.ThroughUrl,
                                            TargetUrl = "portal/page/databases-management"
                                        }
                                    }
                                }
                        }
                    }
                }
            };

            var appFormPage = new Page
            {
                Id = "5d35ce3515066059bc5dfd01",
                Name = "app-form",
                DisplayName = "App Form",
                UrlPath = "portal/page/app-form",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                ShellOptions = new List<ShellOption>
                    {
                        new ShellOption
                        {
                            Key = "entityname",
                            Value = "apps"
                        }
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                            Id = "d69b69db-9850-15aa-08cf-ee6b3c1afa81",
                            Name = "appinfo",
                            DisplayName = "App Info",
                            ConstructionType = SectionContructionType.Standard,
                            ComponentId = "5d3836194d8fa90874135d68",
                            Order = 0,
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
                                EntityName = "databases",
                                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? 
                                "{\"$query\":{\"apps\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
                                : "Select * from apps Where id={{queryparams.id}}"
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
                                ConfirmationText = "Are you sure to create a app?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                        "{\r\n  \"$insert\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"author\": \"{{user.username}}\",\r\n      \"createdDate\": \"ISODate('{{currentISODate()}}')\",\r\n      \"updatedDate\": \"ISODate('{{currentISODate()}}')\"\r\n    }\r\n  }\r\n}"
                                        : "INSERT INTO apps(id, name, displayName, timeSpan, logo, defaultUrl, author, currentVersionNumber, dateCreated, dateModified) Values ({{guid()}}, {{data.name}}, {{data.displayName}}, {{currentTick()}}, {{data.logo}}, {{data.defaultUrl}}, {{user.username}}, {{data.currentVersionNumber}}, {{currentDate()}}, {{currentDate()}})"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Insert new app successfully!",
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
                               ConfirmationText = "Are you sure to update a app?"
                            },
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = ActionType.ExecuteDatabase,
                                DatabaseOptions = new DatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.CoreDatabaseId,
                                    Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                        "{\r\n  \"$update\": {\r\n    \"{{options.entityname}}\": {\r\n      \"$data\": \"{{data}}\",\r\n      \"updatedDate\": \"ISODate('{{currentISODate()}}')\",\r\n      \"$where\": {\r\n        \"_id\": \"ObjectId('{{data.id}}')\"\r\n      }\r\n    }\r\n  }\r\n}"
                                        : "UPDATE apps SET name={{data.name}}, displayName={{data.displayName}}, timeSpan={{currentTick()}}, logo={{data.logo}}, defaultUrl={{data.defaultUrl}}, currentVersionNumber={{data.currentVersionNumber}}, dateModified={{currentDate()}} Where id={{data.id}}"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Update app successfully!",
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
                                        TargetUrl = "portal/page/apps-management",
                                        Condition = "true"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(databaseFormPage);
            versionContext.InsertData(appFormPage);

            var dynamicListsPage = new Page
            {
                Id = "5d4d8adfae5f5b68b811ec20",
                Name = "dynamic-list-management",
                DisplayName = "Dynamic Lists Management",
                UrlPath = "portal/page/dynamic-list-management",
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
                            Name = "dynamiclists",
                            DisplayName = "Dynamic List",
                            ComponentId = "5d4d8adfae5f5b68b811ec1d",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            var standardListsPage = new Page
            {
                Id = "5d4d8adfae5f5b68b811ec21",
                Name = "standard-list-management",
                DisplayName = "Standards Management",
                UrlPath = "portal/page/standard-list-management",
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
                            Name = "standardslist",
                            DisplayName = "Standards List",
                            ComponentId = "5d4d8adfae5f5b68b811ec1e",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            versionContext.InsertData(dynamicListsPage);
            versionContext.InsertData(standardListsPage);
        }
    }
}
