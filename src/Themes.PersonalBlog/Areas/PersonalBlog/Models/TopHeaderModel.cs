using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Abstractions;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models
{
    public class TopHeaderModel : IThemeManifest
    {
        public string SiteName { get; set; }
    }
}
