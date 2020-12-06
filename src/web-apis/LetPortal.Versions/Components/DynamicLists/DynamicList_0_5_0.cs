using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_5_0 : IPortalVersion
    {
        public string VersionNumber => "0.5.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5e1aa91e3c107562acf358b2");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var backupsList = new DynamicList
            {
                Id = "5e1aa91e3c107562acf358b2",
                Name = "backupsList",
                DisplayName = "Backups List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB
                        ? "{ \"$query\": { \"backups\": [ ] } }" : "SELECT * FROM backups"
                    },
                    SourceType = DynamicListSourceType.Database
                },
                ColumnsList = new ColumnsList
                {
                    ColumnDefs = new List<ColumnDef>
                    {
                        new ColumnDef
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
                        new ColumnDef
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
                        new ColumnDef
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
                        new ColumnDef
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
                        new ColumnDef
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
                        new ColumnDef
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

            return Task.CompletedTask;
        }
    }
}
