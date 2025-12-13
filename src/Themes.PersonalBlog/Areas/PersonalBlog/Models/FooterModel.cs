using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Abstractions;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models
{
    public class FooterModel : IThemeManifest
    {
        public string CompanyName { get; set; }
    }
}
