using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Abstractions;

namespace Themes.PersonalBlog
{
    public class PersonalBlogThemeRegistration : IThemeRegistration
    {
        public string Name => "PersonalBlog";

        public string Description => "Personal Blog Theme";

        public string ScreenShotUri => "https://docs.letportal.app/assets/images/logo.png";

        public string Creator => "CLI";

        public IEnumerable<string> AcceptedModules => new List<string>
        {
            "BlogModule"
        };
    }
}
