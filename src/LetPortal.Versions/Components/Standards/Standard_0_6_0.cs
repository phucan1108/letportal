using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions.Components.Standards
{
    public class Standard_0_6_0 : IPortalVersion
    {
        public string VersionNumber => "0.6.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<StandardComponent>("5e79c14931a1754a2cd38cbc");
            versionContext.DeleteData<StandardComponent>("5e79c14931a1754a2cd38cc3");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var profileStandard = new StandardComponent
            {
                Id = "5e79c14931a1754a2cd38cbc",
                Name = "profileForm",
                DisplayName = "Profile Form",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.TwoColumns,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "fullName",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Full Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a full name"
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
                               Value = "fullName"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a full name"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "avatar",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Uploader,                        
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Avatar"
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
                               Value = "avatar"
                           },
                           new Portal.Entities.Pages.ShellOption
                            {
                                Key = "allowfileurl",
                                Value = "true"
                            },
                            new Portal.Entities.Pages.ShellOption
                            {
                                Key = "saveonchange",
                                Value = "false"
                            }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {                               
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.FileMaximumSize,
                                IsActive = true,
                                ValidatorMessage = "File must have size less than {{option}} Mb",
                                ValidatorOption = "5"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.FileExtensions,
                                IsActive = true,
                                ValidatorMessage = "File must have an extension in {{option}}",
                                ValidatorOption = "jpg;jpeg;png;gif"
                            }
                        }
                    }
                }
            };

            var changePasswordStandard = new StandardComponent
            {
                Id = "5e79c14931a1754a2cd38cc3",
                Name = "changePasswordForm",
                DisplayName = "Change Password Form",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.TwoColumns,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "currentPassword",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Password,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Current Password"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a current password"
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
                               Value = "currentPassword"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a current password"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = false
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                                IsActive = false
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Equal,
                                IsActive = false
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                                IsActive = false
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                                IsActive = false
                            }
                        }
                    },
                     new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "newPassword",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Password,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "New password"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a new password"
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
                               Value = "newPassword"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a new password"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                                IsActive = true,
                                ValidatorMessage = "Password requires at least one upper case, one lower case, one number, one special character",
                                ValidatorOption = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MinLength,
                                IsActive = true,
                                ValidatorMessage = "Password requires at least {{option}} characters",
                                ValidatorOption = "6"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "reNewPassword",
                        Order = 3,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Password,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Confirm new password"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Re-enter a new password"
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
                               Value = "reNewPassword"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a password"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum {{option}} characters",
                                ValidatorOption = "50"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                                IsActive = true,
                                ValidatorMessage = "This field must be equal to new password",
                                ValidatorOption = "newPassword"
                            }
                        }
                    }
                }
            };

            VersionUtils.GenerateStandardOptions(profileStandard);
            VersionUtils.GenerateStandardOptions(changePasswordStandard);
            versionContext.InsertData(profileStandard);
            versionContext.InsertData(changePasswordStandard);

            return Task.CompletedTask;
        }
    }
}
