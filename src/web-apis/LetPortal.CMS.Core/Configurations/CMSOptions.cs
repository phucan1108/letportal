using System.Collections.Generic;

namespace LetPortal.CMS.Core.Configurations
{
    public class CMSOptions
    {
        /// <summary>
        /// Currently we got the issue on custom routing with matched all (*)
        /// So we should force starting with chosen theme
        /// </summary>
        public string DefaultTheme { get; set; }

        /// <summary>
        /// We don't support to scan all assemblies because it is so heavy
        /// We need to pick all possible .dll that are CMS Module
        /// </summary>

        public List<string> Modules { get; set; }
    }
}
