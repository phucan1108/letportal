using System.Collections.Generic;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions
{
    public class Constants
    {
        public const string PortalDatabaseId = "5c17b4df7cf5e34530d103b9";
        public const string CoreAppId = "5c162e9005924c1c741bfdc2";
        public const string ServiceManagementDatabaseId = "5dc786a40f4b6b13e0a909f2";
        public const string IdentityDatabaseId = "5e27ea649d474848acec4510";

        public const string GeneralInsertWorkflowId = "5d3161aee7fefb62d805bf6f";
        public const string GeneralUpdateWorkflowId = "5d31634fe7fefb62d805bf74";
        public const string GeneralDeleteWorkflowId = "5d316607e7fefb62d805bf79";

        public static List<ShellOption> TreeOptions(
            string inStructure = "nest", 
            string outStructure = "nest", 
            string displayNameField = "displayName",
            string inChildren = "sub",
            string outChildren = "sub",
            string nodeIdField = "_id",
            string nodeParentField = "parentId",
            bool allowGenerateId = true)
        {
            return new List<ShellOption>
            {
                new ShellOption
                {
                    Key = "indatastructure",
                    Value = inStructure
                },
                new ShellOption
                {
                    Key = "outdatastructure",
                    Value = outStructure
                },
                new ShellOption
                {
                    Key = "displayname",
                    Value = displayNameField
                },
                new ShellOption
                {
                    Key = "allowgenerateid",
                    Value = allowGenerateId ? "true" : "false"
                },
                new ShellOption
                {
                    Key = "inchildren",
                    Value = inChildren
                },
                new ShellOption
                {
                    Key = "outchildren",
                    Value = outChildren
                },
                new ShellOption
                {
                    Key = "nodeidfield",
                    Value = nodeIdField
                },
                new ShellOption
                {
                    Key = "nodeparentfield",
                    Value = nodeParentField
                }
            };
        }

        public static List<ShellOption> ControlOptions(
            string label,
            string placeHolder,
            string disabled,
            string hidden,
            string bindName,
            string rendered,
            List<ShellOption> combinedOptions = null
            )
        {
            var options =  new List<ShellOption>
            {
                new ShellOption
                {
                    Key = "label",
                    Value = label
                },
                new ShellOption
                {
                    Key = "placeholder",
                    Value = placeHolder
                },
                new ShellOption
                {
                    Key = "disabled",
                    Value = disabled
                },
                new ShellOption
                {
                    Key = "hidden",
                    Value = hidden
                },
                new ShellOption
                {
                    Key = "rendered",
                    Value = rendered
                },
                new ShellOption
                {
                    Key = "bindname",
                    Value = bindName
                }
            };

            if(combinedOptions != null)
            {
                options.AddRange(combinedOptions);
            }

            return options;
        }

        public static List<ShellOption> DynamicListOptions()
        {
            return new List<ShellOption>
            {
                new ShellOption
                {
                    Key = "sizeoptions",
                    Value = "[10,20,50,100,200]"
                },
                new ShellOption
                {
                    Key = "defaultpagesize",
                    Value = "10"
                },
                new ShellOption
                {
                    Key = "fetchfirsttime",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "maximumcolumns",
                    Value = "6"
                },
                new ShellOption
                {
                    Key = "enablesearch",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "enableadvancedsearch",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "enablepagination",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "enableexportexcel",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "maximumclientexport",
                    Value = "100"
                },
                new ShellOption
                {
                    Key = "allowexporthiddenfields",
                    Value = "false"
                }
            };
        }

        public static List<ShellOption> ChartOptions()
        {
            return new List<ShellOption>
            {
                new ShellOption
                {
                    Key = "allowrealtime",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "timetorefresh",
                    Value = "60"
                },
                new ShellOption
                {
                    Key = "comparerealtimefield",
                    Value = ""
                },
                new ShellOption
                {
                    Key = "datarange",
                    Value = ""
                },
                new ShellOption
                {
                    Key = "xformatdate",
                    Value = ""
                },
                new ShellOption
                {
                    Key = "colors",
                    Value = "[\"horizon\"]"
                }
            };
        }

        public static List<ShellOption> UploadOptions()
        {
            return new List<ShellOption>
            {
                new ShellOption
                {
                    Key = "allowfileurl",
                    Value = "true"
                },
                new ShellOption
                {
                    Key = "saveonchange",
                    Value = "false"
                }
            };
        }
    }
}
