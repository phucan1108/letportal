using System;
using System.Collections.Generic;
using System.Text;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models
{
    public class MenuModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public IEnumerable<MenuModel> Sub { get; set; }
    }
}
