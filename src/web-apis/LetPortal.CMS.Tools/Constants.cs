using System.Collections.Generic;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.CMS.Tools
{
    public class Constants
    {
        public const string CMS_APP_ID = "5f23f7d6b8f393672ce21029";

        public const string CMS_DATABASE_ID = "5f33c1ddedf7b3de91e106d8";

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
    }
}
