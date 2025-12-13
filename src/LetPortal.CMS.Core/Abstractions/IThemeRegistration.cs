using System.Collections;
using System.Collections.Generic;

namespace LetPortal.CMS.Core.Abstractions
{
    public interface IThemeRegistration
    {
        public string Name { get; }

        public string Description { get; }

        public string ScreenShotUri { get; }

        public string Creator { get; }

        public IEnumerable<string> AcceptedModules { get; }
    }
}
