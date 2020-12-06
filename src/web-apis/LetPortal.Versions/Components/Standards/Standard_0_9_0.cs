using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions.Components.Standards
{
    public class Standard_0_9_0 : IPortalVersion
    {
        public string VersionNumber => "0.9.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<StandardComponent>("5f6f751c118d1754ac475aa1");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var appMenu = new StandardComponent
            {
                Id = "5f6f751c118d1754ac475aa1",
                Name = "appmenu",
                DisplayName = "App Menu",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.OneColumn,
                Type = StandardType.Tree,
                Options = Constants.TreeOptions(inChildren: "subMenus", outChildren: "subMenus"),
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "displayName",
                        IsActive = true,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = Constants.ControlOptions("Menu name", "Enter a menu name", "false", "false", "displayName", "true"),
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a menu name"
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
                        Name = "icon",
                        IsActive = true,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.IconPicker,
                        Options = Constants.ControlOptions("Menu Icon", "", "false", "false", "icon", "true"),
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please choose one icon"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "url",
                        IsActive = true,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = Constants.ControlOptions("Menu Url", "Enter a menu url", "false", "false", "url", "true"),
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a url"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "hide",
                        IsActive = true,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Slide,
                        Options = Constants.ControlOptions("Hide", "", "false", "false", "hide", "true"),
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "availableUrl",
                        IsActive = true,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Select,
                        Options = Constants.ControlOptions("Choose url", "", "false", "false", "availableUrl", "true",
                            new List<Portal.Entities.Pages.ShellOption>
                            {
                                new Portal.Entities.Pages.ShellOption
                                {
                                    Key = "defaultvalue",
                                    Value = ""
                                },
                                new Portal.Entities.Pages.ShellOption
                                {
                                    Key = "multiple",
                                    Value = "false"
                                }
                            }),
                        DatasourceOptions = new Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = Portal.Entities.Shared.DatasourceControlType.WebService,
                            HttpServiceOptions = new Portal.Entities.Shared.HttpServiceOptions
                            {
                                HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/apps/{{queryparams.appId}}/urls",
                                HttpMethod = "GET",
                                HttpSuccessCode = "200",
                                JsonBody = "{}",
                                OutputProjection = "name=name;value=url"
                            }
                        }
                    }
                }
            };

            VersionUtils.GenerateControlEvents(appMenu);

            versionContext.InsertData(appMenu);

            return Task.CompletedTask;
        }
    }
}
