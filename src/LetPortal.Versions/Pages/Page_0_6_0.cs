using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_6_0 : IPortalVersion
    {
        public string VersionNumber => "0.6.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5e79c14931a1754a2cd38cbd");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var userInfoPage = new Page
            {
                Id = "5e79c14931a1754a2cd38cbd",
                Name = "user-info",
                DisplayName = "User Info",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/user-info",
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
                            Id = "5e79c14931a1754a2cd38cbe",
                            Name = "profileform",
                            DisplayName = "User Profile",
                            ComponentId = "5e79c14931a1754a2cd38cbc",
                            ConstructionType = SectionContructionType.Standard,
                            Order = 0,
                            SectionDatasource = new SectionDatasource
                            {
                                DatasourceBindName = "data"
                            }
                        },
                        new PageSection
                        {
                            Id = "5e79c14931a1754a2cd38cc5",
                            Name = "changepassword",
                            DisplayName = "Change Password",
                            ComponentId = "5e79c14931a1754a2cd38cc3",
                            ConstructionType = SectionContructionType.Standard,
                            Order = 1,
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
                        Id = "5e79c14931a1754a2cd38cbc",
                        IsActive = true,
                        Name = "data",
                        TriggerCondition = "!!user",
                        Options = new Portal.Entities.Shared.DatasourceOptions
                        {
                            DatasourceStaticOptions = new Portal.Entities.Shared.DatasourceStaticOptions
                            {
                                JsonResource = "{\r\n  \"fullName\": \"{{user.fullName}}\",\r\n  \"avatar\": \"{{user.avatar}}\"\r\n}"
                            },
                            Type = Portal.Entities.Shared.DatasourceControlType.StaticResource
                        }
                    }
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "5e7a1b52f85d128568d07737",
                        Name = "Update Profile",
                        Icon = "save",
                        Color = "primary",
                        IsRequiredValidation = true,
                        AllowHidden = "false",
                        PlaceSectionId = "5e79c14931a1754a2cd38cbe",
                        ButtonOptions = new Portal.Entities.Shared.ButtonOptions
                        {
                            ActionCommandOptions = new Portal.Entities.Shared.ActionCommandOptions
                            {
                                ActionType = Portal.Entities.Shared.ActionType.CallHttpService,
                                HttpServiceOptions = new Portal.Entities.Shared.HttpServiceOptions
                                {
                                    HttpServiceUrl = "{{configs.identityBaseEndpoint}}/api/profiles/update",
                                    HttpMethod = "Post",
                                    HttpSuccessCode = "200;204",
                                    JsonBody = "{\r\n  \"fullName\": \"{{data.fullName}}\",\r\n  \"avatar\": \"{{data.avatar}}\"\r\n}"
                                },
                                IsEnable = true,
                                ConfirmationOptions = new Portal.Entities.Shared.ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to update the profile?"
                                },
                                NotificationOptions = new Portal.Entities.Shared.NotificationOptions
                                {
                                    CompleteMessage = "Your profile has been updated, please sign out for making its effectively.",
                                    FailedMessage = "Oops! Something went wrong, please try again."
                                }
                            }
                        }
                    },
                    new PageButton
                    {
                        Id = "5e7a1b52f85d128568d07737",
                        Name = "Change Password",
                        Icon = "lock_open",
                        Color = "warn",
                        AllowHidden = "false",
                        IsRequiredValidation = true,
                        PlaceSectionId = "5e79c14931a1754a2cd38cc5",
                        ButtonOptions = new Portal.Entities.Shared.ButtonOptions
                        {
                            ActionCommandOptions = new Portal.Entities.Shared.ActionCommandOptions
                            {
                                ActionType = Portal.Entities.Shared.ActionType.CallHttpService,
                                HttpServiceOptions = new Portal.Entities.Shared.HttpServiceOptions
                                {
                                    HttpServiceUrl = "{{configs.identityBaseEndpoint}}/api/accounts/change-password",
                                    HttpMethod = "Post",
                                    HttpSuccessCode = "200;204",
                                    JsonBody = "{\r\n  \"currentPassword\": \"{{data.currentPassword}}\",\r\n  \"newPassword\": \"{{data.newPassword}}\",\r\n  \"reNewPassword\": \"{{data.reNewPassword}}\"\r\n}"
                                },
                                IsEnable = true,
                                ConfirmationOptions = new Portal.Entities.Shared.ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to change the password?"
                                },
                                NotificationOptions = new Portal.Entities.Shared.NotificationOptions
                                {
                                    CompleteMessage = "Your password has been updated, please sign out for making its effectively.",
                                    FailedMessage = "Oops! Something went wrong, please try again."
                                }
                            }
                        }
                    },
                }
            };

            versionContext.InsertData(userInfoPage);
            return Task.CompletedTask;
        }
    }
}
