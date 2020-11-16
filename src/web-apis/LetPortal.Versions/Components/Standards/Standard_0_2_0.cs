using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions.Components
{
    public class Standard_0_2_0 : IPortalVersion
    {
        public string VersionNumber => "0.2.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<StandardComponent>("5d615e501773a96ee8eb5ed3");
            versionContext.DeleteData<StandardComponent>("5d6222ec3aae6a79ecf035e9");
            versionContext.DeleteData<StandardComponent>("5d63423dbaac7d4790b7d300");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var userFormStandard = new StandardComponent
            {
                Id = "5d615e501773a96ee8eb5ed3",
                Name = "userform",
                DisplayName = "User Form",
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
                        Name = "username",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Label,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Username"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a username"
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
                               Value = "username"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "email",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Email,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Email"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a email"
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
                               Value = "email"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a email"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                                IsActive = true,
                                ValidatorMessage = "Please enter a correct email format"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "isConfirmedEmail",
                        Order = 3,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Checkbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Confirmed Email"
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
                               Value = "isConfirmedEmail"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "isLockoutEnabled",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Checkbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Lockout Enabled"
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
                               Value = "isLockoutEnabled"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "lockoutEndDate",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Label,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Lockout End Date"
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
                               Value = "lockoutEndDate"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "roles",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.AutoComplete,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Roles"
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
                               Value = "roles"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "multiple",
                               Value = "true"
                           }
                        },
                        DatasourceOptions = new Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = Portal.Entities.Shared.DatasourceControlType.Database,
                            DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                            {
                                DatabaseConnectionId = Constants.IdentityDatabaseId,
                                EntityName = "roles",
                                Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                "{\r\n  \"$query\": {\r\n    \"roles\": [\r\n      {\r\n        \"$project\": {\r\n          \"name\": \"$displayName\",\r\n          \"value\": \"$name\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select `displayName` as name, name as value from `roles`" : "Select \"displayName\" as name, name as value from roles")
                            }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please select at least one role",
                                ValidatorOption = ""
                            }
                        }
                    }
                }
            };

            var registerNewUserFormStandard = new StandardComponent
            {
                Id = "5d6222ec3aae6a79ecf035e9",
                Name = "registerForm",
                DisplayName = "Register Form",
                AppId = Constants.CoreAppId,
                LayoutType = PageSectionLayoutType.OneColumn,
                Controls = new List<Portal.Entities.SectionParts.Controls.PageControl>
                {
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "username",
                        Order = 1,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Textbox,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Username"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a username"
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
                               Value = "username"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a username"
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
                                ValidatorMessage = "This field requires maximum 50 characters",
                                ValidatorOption = "50"
                            }
                        },
                        AsyncValidators = new List<Portal.Entities.Components.Controls.PageControlAsyncValidator>
                        {
                            new Portal.Entities.Components.Controls.PageControlAsyncValidator
                            {
                                ValidatorName = "uniquename",
                                IsActive = true,
                                ValidatorMessage = "This username is already existed. Please choose another name.",
                                AsyncValidatorOptions = new Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                                {
                                    EvaluatedExpression = "response.result == null",
                                    DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                                    {
                                      DatabaseConnectionId = Constants.IdentityDatabaseId,
                                      Query = "{\r\n  \"$query\": {\r\n    \"users\": [\r\n      {\r\n        \"$match\": {\r\n          \"username\": \"{{data.username}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                    },
                                    ValidatorType = Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator
                                }
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "password",
                        Order = 2,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Password,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Password"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a password"
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
                               Value = "password"
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
                        Name = "repassword",
                        Order = 3,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Password,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Confirm password"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a confirm password"
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
                               Value = "repassword"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a confirm password"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 50 characters",
                                ValidatorOption = "50"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.EqualTo,
                                IsActive = true,
                                ValidatorMessage = "This field must be equal to {{option}}",
                                ValidatorOption = "password"
                            }
                        }
                    },
                    new Portal.Entities.SectionParts.Controls.PageControl
                    {
                        Name = "email",
                        Order = 4,
                        Type = Portal.Entities.SectionParts.Controls.ControlType.Email,
                        Options = new List<Portal.Entities.Pages.ShellOption>
                        {
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "label",
                               Value = "Email"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a email"
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
                               Value = "email"
                           }
                        },
                        Validators = new List<Portal.Entities.SectionParts.Controls.PageControlValidator>
                        {
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Required,
                                IsActive = true,
                                ValidatorMessage = "Please enter a email"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 250 characters",
                                ValidatorOption = "250"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Email,
                                IsActive = true,
                                ValidatorMessage = "This field must be email format"
                            }
                        },
                         AsyncValidators = new List<Portal.Entities.Components.Controls.PageControlAsyncValidator>
                        {
                            new Portal.Entities.Components.Controls.PageControlAsyncValidator
                            {
                                ValidatorName = "uniqueemail",
                                IsActive = true,
                                ValidatorMessage = "This email is already existed. Please choose another email.",
                                AsyncValidatorOptions = new Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                                {
                                    EvaluatedExpression = "response.result == null",
                                    DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                                    {
                                      DatabaseConnectionId = Constants.IdentityDatabaseId,
                                      Query = "{\r\n  \"$query\": {\r\n    \"users\": [\r\n      {\r\n        \"$match\": {\r\n          \"email\": \"{{data.emaill}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                    },
                                    ValidatorType = Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator
                                }
                            }
                        }
                    }
                }
            };

            var roleStandard = new StandardComponent
            {
                Id = "5d63423dbaac7d4790b7d300",
                Name = "roleForm",
                DisplayName = "Role Form",
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
                               Value = "Role Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a role name"
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
                                ValidatorMessage = "Please enter a name"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.MaxLength,
                                IsActive = true,
                                ValidatorMessage = "This field requires maximum 50 characters",
                                ValidatorOption = "50"
                            },
                            new Portal.Entities.SectionParts.Controls.PageControlValidator
                            {
                                ValidatorType = Portal.Entities.SectionParts.Controls.ValidatorType.Regex,
                                IsActive = true,
                                ValidatorMessage = "Please enter only characters",
                                ValidatorOption = "[A-Za-z]*"
                            }
                        },
                        AsyncValidators = new List<Portal.Entities.Components.Controls.PageControlAsyncValidator>
                        {
                            new Portal.Entities.Components.Controls.PageControlAsyncValidator
                            {
                                ValidatorName = "uniquename",
                                IsActive = true,
                                ValidatorMessage = "This role is already existed. Please choose another name.",
                                AsyncValidatorOptions = new Portal.Entities.Components.Controls.ControlAsyncValidatorOptions
                                {
                                    EvaluatedExpression = "response.result == null",
                                    DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                                    {
                                      DatabaseConnectionId = Constants.IdentityDatabaseId,
                                      Query = "{\r\n  \"$query\": {\r\n    \"roles\": [\r\n      {\r\n        \"$match\": {\r\n          \"name\": \"{{data.name}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
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
                               Value = "Role Display Name"
                           },
                           new Portal.Entities.Pages.ShellOption
                           {
                               Key = "placeholder",
                               Value = "Enter a display name"
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
                                ValidatorMessage = "This field requires maximum 50 characters",
                                ValidatorOption = "50"
                            }
                        }
                    }
                }
            };

            VersionUtils.GenerateControlEvents(userFormStandard);
            VersionUtils.GenerateControlEvents(registerNewUserFormStandard);
            VersionUtils.GenerateControlEvents(roleStandard);

            VersionUtils.GenerateStandardOptions(userFormStandard);
            VersionUtils.GenerateStandardOptions(registerNewUserFormStandard);
            VersionUtils.GenerateStandardOptions(roleStandard);

            versionContext.InsertData(userFormStandard);
            versionContext.InsertData(registerNewUserFormStandard);
            versionContext.InsertData(roleStandard);

            return Task.CompletedTask;
        }
    }
}
