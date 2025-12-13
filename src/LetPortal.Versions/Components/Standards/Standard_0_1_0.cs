using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions.Components.Standards
{
    public class Standard_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<StandardComponent>("5d25f6abe88d28422061cfaf");
            versionContext.DeleteData<StandardComponent>("5d3836194d8fa90874135d68");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var databaseStandard = new StandardComponent
            {
                Id = "5d25f6abe88d28422061cfaf",
                Name = "databaseForm",
                DisplayName = "Database Form",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.OneColumn,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "id",
                        Order = 0,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "id"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "id"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "true"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "true"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "id"
                           }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "name",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "name"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a database name"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                                IsActive = true,
                                ValidatorMessage = "Please enter only characters and numberic",
                                ValidatorOption = "[A-Za-z0-9]*"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        },
                        AsyncValidators = new List<Portal.Entities.Components.Controls.PageControlAsyncValidator>
                        {
                            new Portal.Entities.Components.Controls.PageControlAsyncValidator
                            {
                                ValidatorName = "uniquename",
                                IsActive = true,
                                ValidatorMessage = "This database name is already existed. Please choose another name.",
                                AsyncValidatorOptions = new Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                                {
                                    EvaluatedExpression = "response.result == null",
                                    DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                                    {
                                      DatabaseConnectionId = Constants.PortalDatabaseId,
                                      Query = "{\r\n  \"$query\": {\r\n    \"databases\": [\r\n      {\r\n        \"$match\": {\r\n          \"name\": \"{{data.name}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                    },
                                    ValidatorType = Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator
                                }
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "dataSource",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Datasource"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Datasource"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "dataSource"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a datasource"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "connectionString",
                        Order = 3,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Connection String"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Connection String"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "connectionString"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a connection string"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 500 characters",
                                ValidatorOption = "500"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "databaseConnectionType",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Select,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Connection Type"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = ""
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "databaseConnectionType"
                           }
                        },
                        DatasourceOptions = new Portal.Entities.Shared.DatasourceOptions
                        {
                            DatasourceStaticOptions = new Portal.Entities.Shared.DatasourceStaticOptions
                            {
                                JsonResource = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}, {\"name\":\"PostgreSQL\",\"value\":\"postgresql\"}, {\"name\":\"MySQL\",\"value\":\"mysql\"}]"
                            }
                        },
                         Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please choose one connection type"
                            }
                        }
                    }
                }
            };

            var appStandard = new StandardComponent
            {
                Id = "5d3836194d8fa90874135d68",
                Name = "appForm",
                DisplayName = "App Form",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.OneColumn,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "id",
                        Order = 0,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Id"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = ""
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "true"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "true"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "id"
                           }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "name",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "!!queryparams.id"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "name"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a app name"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                                IsActive = true,
                                ValidatorMessage = "Please enter only characters and numberic",
                                ValidatorOption = "[A-Za-z0-9]*"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        },
                        AsyncValidators = new List<Portal.Entities.Components.Controls.PageControlAsyncValidator>
                        {
                            new Portal.Entities.Components.Controls.PageControlAsyncValidator
                            {
                                ValidatorName = "uniquename",
                                IsActive = true,
                                ValidatorMessage = "This app name is already existed. Please choose another name.",
                                AsyncValidatorOptions = new Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                                {
                                    EvaluatedExpression = "response.result == null",
                                    DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                                    {
                                      DatabaseConnectionId = Constants.PortalDatabaseId,
                                      Query = "{\r\n  \"$query\": {\r\n    \"apps\": [\r\n      {\r\n        \"$match\": {\r\n          \"name\": \"{{data.name}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                    },
                                    ValidatorType = Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator
                                }
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "displayName",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Display Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Display Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "displayName"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a display name"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "defaultUrl",
                        Order = 3,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Default Url"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Default Url"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "defaultUrl"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a default url"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "currentVersionNumber",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Version"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "App Version"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "currentVersionNumber"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a version number"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 50 characters",
                                ValidatorOption = "50"
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "logo",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.IconPicker,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Icon"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Icon"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "disabled",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "hidden",
                               Value = "false"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "bindname",
                               Value = "logo"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please choose one icon"
                            }
                        }
                    }
                }
            };

            VersionUtils.GenerateControlEvents(databaseStandard);
            VersionUtils.GenerateControlEvents(appStandard);

            VersionUtils.GenerateStandardOptions(databaseStandard);
            VersionUtils.GenerateStandardOptions(appStandard);
            versionContext.InsertData(databaseStandard);
            versionContext.InsertData(appStandard);

            return Task.CompletedTask;
        }
    }
}
