using System.Collections.Generic;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_5_0 : IPortalVersion
    {
        public string VersionNumber => "0.5.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5e1aa91e3c107562acf358b2");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var backupsList = new DynamicList
            {
                Id = "5e1aa91e3c107562acf358b2",
                Name = "backupsList",
                DisplayName = "Backups List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB
                        ? "{ \"$query\": { \"backups\": [ ] } }" : "SELECT * FROM backups"
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
                            Name = "name",
                            DisplayName = "Backup Name",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        },
                        new ColumndDef
                        {
                            Name = "description",
                            DisplayName = "Description",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        },
                        new ColumndDef
                        {
                            Name = "creator",
                            DisplayName = "Creator",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 3
                        },
                        new ColumndDef
                        {
                            Name = "createdDate",
                            DisplayName = "Created Date",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.DatePicker,
                                AllowTextSearch = false
                            },
                            Order = 4
                        },
                        new ColumndDef
                        {
                            Name = "downloadableUrl",
                            DisplayName = "downloadableUrl",
                            IsHidden = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },
                            Order = 5
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
                                   RedirectUrl = "portal/builder/backup"
                                }
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "upload",
                            DisplayName = "Restore",
                            Icon = "restore",
                            Color = "primary",
                            CommandPositionType = CommandPositionType.OutList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/backup/upload"
                                }
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "download",
                            DisplayName = "Download File",
                            Icon = "save_alt",
                            Color = "accent",
                            CommandPositionType = CommandPositionType.InList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = false,
                                   RedirectUrl = "{{data.downloadableUrl}}"
                                },
                                IsEnable = true
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "restorepoint",
                            DisplayName = "Preview&Restore",
                            Icon = "restore",
                            Color = "primary",
                            CommandPositionType = CommandPositionType.InList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/backup/restore/{{data.id}}"
                                },
                                IsEnable = true
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(backupsList);
        }
    }
}
