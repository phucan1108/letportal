using System.Collections.Generic;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions.Components
{
    public class Standard_0_0_1 : IPortalVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<StandardComponent>("5d25f6abe88d28422061cfaf");
            versionContext.DeleteData<StandardComponent>("5d3836194d8fa90874135d68");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseStandard = new StandardComponent
            {
                Id = "5d25f6abe88d28422061cfaf",
                Name = "database-form",
                DisplayName = "Database Form",
                LayoutType = PageSectionLayoutType.OneColumn,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Id = "be0bbde4-b189-9e7d-95ab-98af8e681559",
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
                               Key = "tooltip",
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
                        Id = "b1b51c52-1312-c5ea-2e5a-8296392da949",
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
                               Key = "tooltip",
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
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Id = "da8c538a-3791-596d-93e2-ec01cbe8b198",
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
                               Key = "tooltip",
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
                        Id = "b1b51c52-1312-c5ea-2e5a-8296392da949",
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
                               Key = "tooltip",
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
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec2",
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
                               Key = "tooltip",
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
                Name = "app-form",
                DisplayName = "App Form",
                LayoutType = PageSectionLayoutType.OneColumn,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec1",
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
                               Key = "tooltip",
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
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec2",
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
                               Key = "tooltip",
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
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec3",
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
                               Key = "tooltip",
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
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec4",
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
                               Key = "tooltip",
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
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8232",
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
                               Key = "tooltip",
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
                        Id = "55d8d892-bbe0-f8c0-b3c6-b18b186b8ec1",
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
                               Key = "tooltip",
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
            versionContext.InsertData(databaseStandard);
            versionContext.InsertData(appStandard);
        }
    }
}
