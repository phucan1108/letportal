using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_8_0 : IPortalVersion
    {
        public string VersionNumber => "0.8.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5ea80612bf1ac062f89f6f54");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var localizationList = new DynamicList
            {
                Id = "5ea80612bf1ac062f89f6f54",
                Name = "localizationsList",
                DisplayName = "Localization List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB
                       ? "{\r\n  \"$query\": {\r\n    \"localizations\":[\r\n      {\r\n        \"$match\": {\r\n          \"pageId\": \"{{queryparams.pageId}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}" : "SELECT * FROM localizations"
                    },
                    SourceType = DynamicListSourceType.Database
                },
                ColumnsList = new ColumnsList
                {
                    ColumndDefs = new List<ColumndDef>
                    {
                        new ColumndDef
                        {
                            Name = "id",
                            DisplayName = "Id",
                            IsHidden = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },
                            Order = 0
                        },
                        new ColumndDef
                        {
                            Name = "pageId",
                            DisplayName = "pageId",
                            IsHidden = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },
                            Order = 0
                        },
                        new ColumndDef
                        {
                            Name = "localeId",
                            DisplayName = "Locale Id",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        }
                    }
                },
                CommandsList = new CommandsList
                {
                    CommandButtonsInList = new List<CommandButtonInList>
                    {
                        new CommandButtonInList
                        {
                            Name = "create",
                            DisplayName = "Create",
                            Icon = "create",
                            Color = "primary",
                            CommandPositionType = CommandPositionType.OutList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/localization?pageId={{queryparams.pageId}}"
                                }
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "edit",
                            DisplayName = "Edit",
                            Icon = "create",
                            Color = "warn",
                            CommandPositionType = CommandPositionType.InList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/localization/{{data.id}}"
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(localizationList);
        }
    }
}
